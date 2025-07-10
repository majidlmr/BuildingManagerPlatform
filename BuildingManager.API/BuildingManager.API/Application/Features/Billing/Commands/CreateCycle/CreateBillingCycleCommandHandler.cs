using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Enums; // Ensure Enums are used
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Billing.Commands.CreateCycle;

/// <summary>
/// پردازشگر دستور ایجاد چرخه حسابداری با استفاده از الگوی استراتژی.
/// </summary>
public class CreateBillingCycleCommandHandler : IRequestHandler<CreateBillingCycleCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;
    private readonly IEnumerable<IChargeCalculationStrategy> _chargeStrategies;

    public CreateBillingCycleCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService,
        IEnumerable<IChargeCalculationStrategy> chargeStrategies)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
        _chargeStrategies = chargeStrategies;
    }

    public async Task<int> Handle(CreateBillingCycleCommand request, CancellationToken cancellationToken)
    {
        // Assuming request.BuildingId is now request.BlockId or similar if Command was updated
        // For now, using request.BuildingId and will map to BlockId conceptually
        var blockId = request.BuildingId; // This should ideally be BlockId in the command

        var canCreate = await _authorizationService.HasPermissionAsync(request.RequestingUserId, blockId, "Billing.CreateCycle", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه ایجاد چرخه مالی برای این بلوک را ندارید.");
        }

        var block = await _context.Blocks // Changed from Buildings to Blocks
            .Include(b => b.Units)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == blockId, cancellationToken);
        if (block == null) throw new NotFoundException("بلوک یافت نشد.");

        var expensesInCycle = await _context.Expenses
            .Where(e => e.BlockId == blockId && e.ExpenseDate >= request.StartDate && e.ExpenseDate <= request.EndDate) // Assuming Expense has BlockId
            .ToListAsync(cancellationToken);

        var activeAssignments = await _context.UnitAssignments // Changed from ResidentAssignments
            .Include(ua => ua.Unit) // Ensure Unit is included for BlockId check
            .Where(ua => ua.Unit.BlockId == blockId && ua.AssignmentStatus == UnitAssignmentStatus.Active) // Assuming UnitAssignmentStatus enum
            .ToDictionaryAsync(ua => ua.UnitId, ua => ua.UserId, cancellationToken); // UserId is the resident

        var strategy = _chargeStrategies.FirstOrDefault(s => s.Name == block.ChargeCalculationStrategyName); // Assuming Block has ChargeCalculationStrategyName
        if (strategy == null)
        {
            throw new InvalidOperationException($"استراتژی محاسبه شارژ با نام '{block.ChargeCalculationStrategyName}' یافت نشد.");
        }

        var expenseDues = strategy.CalculateDues(expensesInCycle, block.Units.ToList());

        var billingCycle = new BillingCycle
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            BlockId = blockId, // Assuming BillingCycle has BlockId
            CreatedAt = DateTime.UtcNow
            // Other properties for BillingCycle...
        };
        await _context.BillingCycles.AddAsync(billingCycle, cancellationToken);
        // It's good practice to SaveChanges here to get billingCycle.Id if needed immediately,
        // or ensure it's handled by the UnitOfWork later.

        var invoicesToCreate = new List<Invoice>();
        foreach (var unit in block.Units)
        {
            if (!activeAssignments.TryGetValue(unit.Id, out var residentUserId)) continue;

            var invoiceItems = new List<InvoiceItem>();

            if (request.DefaultChargePerUnit > 0)
            {
                // Assuming InvoiceItemType.MonthlyCharge exists
                invoiceItems.Add(new InvoiceItem { /*Type = InvoiceItemType.MonthlyCharge,*/ Description = "شارژ ثابت ماهانه", Amount = request.DefaultChargePerUnit, UnitPrice = request.DefaultChargePerUnit, Quantity = 1, TotalAmount = request.DefaultChargePerUnit });
            }

            if (expenseDues.TryGetValue(unit.Id, out var dues))
            {
                // dues should be List<InvoiceItem> or similar structure
                // invoiceItems.AddRange(dues); // This was causing issues if dues was not List<InvoiceItem>
                 foreach (var dueItem in dues) // Assuming dues is a collection of items that can be mapped to InvoiceItem
                 {
                    // Adapt this mapping based on the actual structure of 'dueItem'
                    invoiceItems.Add(new InvoiceItem { Description = dueItem.Description, Amount = dueItem.Amount, UnitPrice = dueItem.Amount, Quantity = 1, TotalAmount = dueItem.Amount });
                 }
            }

            if (invoiceItems.Any())
            {
                invoicesToCreate.Add(new Invoice
                {
                    ComplexId = block.ParentComplexId,
                    BlockId = block.Id,
                    UnitId = unit.Id,
                    UserId = residentUserId,
                    BillingCycleId = billingCycle.Id, // Assign after BillingCycle is saved or ensure EF handles it
                    InvoiceType = InvoiceType.BuildingCharge, // Defaulting to BuildingCharge
                    Amount = invoiceItems.Sum(item => item.Amount),
                    IssueDate = DateTime.UtcNow, // Or request.EndDate or specific logic
                    DueDate = request.EndDate,
                    PaymentDate = null,
                    Status = InvoiceStatus.Unpaid,
                    Description = $"شارژ دوره {request.Name} برای واحد {unit.UnitNumber}",
                    Items = invoiceItems,
                    CreatedAt = DateTime.UtcNow // Already set by default in Invoice entity if configured
                });
            }
        }

        if (invoicesToCreate.Any())
        {
            await _context.Invoices.AddRangeAsync(invoicesToCreate, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return billingCycle.Id; // Return the generated Id
    }
}
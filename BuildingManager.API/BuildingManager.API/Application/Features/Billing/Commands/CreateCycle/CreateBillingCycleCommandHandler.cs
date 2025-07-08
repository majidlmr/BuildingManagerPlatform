using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Enums;
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
        IEnumerable<IChargeCalculationStrategy> chargeStrategies) // 👈 تزریق تمام استراتژی‌های موجود
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
        _chargeStrategies = chargeStrategies;
    }

    public async Task<int> Handle(CreateBillingCycleCommand request, CancellationToken cancellationToken)
    {
        var canCreate = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Billing.CreateCycle", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه ایجاد چرخه مالی برای این ساختمان را ندارید.");
        }

        var building = await _context.Buildings
            .Include(b => b.Units)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BuildingId, cancellationToken);
        if (building == null) throw new NotFoundException("ساختمان یافت نشد.");

        var expensesInCycle = await _context.Expenses
            .Where(e => e.BuildingId == request.BuildingId && e.ExpenseDate >= request.StartDate && e.ExpenseDate <= request.EndDate)
            .ToListAsync(cancellationToken);

        var activeAssignments = await _context.ResidentAssignments
            .Where(ra => ra.Unit.BuildingId == request.BuildingId && ra.IsActive)
            .ToDictionaryAsync(ra => ra.UnitId, ra => ra.ResidentUserId, cancellationToken);

        // ✅ انتخاب استراتژی محاسبه بر اساس تنظیمات ساختمان
        var strategy = _chargeStrategies.FirstOrDefault(s => s.Name == building.ChargeCalculationStrategy);
        if (strategy == null)
        {
            throw new InvalidOperationException($"استراتژی محاسبه شارژ با نام '{building.ChargeCalculationStrategy}' یافت نشد.");
        }

        // ✅ اجرای استراتژی انتخاب شده برای محاسبه سهم هر واحد از هزینه‌ها
        var expenseDues = strategy.CalculateDues(expensesInCycle, building.Units);

        var billingCycle = new BillingCycle { /* ... */ };
        await _context.BillingCycles.AddAsync(billingCycle, cancellationToken);

        var invoicesToCreate = new List<Invoice>();
        foreach (var unit in building.Units)
        {
            if (!activeAssignments.TryGetValue(unit.Id, out var residentId)) continue;

            var invoiceItems = new List<InvoiceItem>();

            // افزودن شارژ ثابت
            if (request.DefaultChargePerUnit > 0)
            {
                invoiceItems.Add(new InvoiceItem { Type = InvoiceItemType.MonthlyCharge, Description = "شارژ ثابت ماهانه", Amount = request.DefaultChargePerUnit });
            }

            // افزودن سهم محاسبه شده از هزینه‌ها توسط استراتژی
            if (expenseDues.TryGetValue(unit.Id, out var dues))
            {
                invoiceItems.AddRange(dues);
            }

            if (invoiceItems.Any())
            {
                invoicesToCreate.Add(new Invoice
                {
                    // ... تخصیص مقادیر صورتحساب
                    Amount = invoiceItems.Sum(item => item.Amount),
                    Items = invoiceItems,
                    // ...
                });
            }
        }

        if (invoicesToCreate.Any())
        {
            await _context.Invoices.AddRangeAsync(invoicesToCreate, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return billingCycle.Id;
    }
}
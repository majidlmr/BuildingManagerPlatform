using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Financials.Commands.CreateExpense;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService; // تزریق سرویس دسترسی

    public CreateExpenseCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService; // مقداردهی در سازنده
    }

    public async Task<int> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی متمرکز دسترسی با استفاده از سرویس
        var canCreate = await _authorizationService.HasPermissionAsync(request.RecordedByUserId, request.BuildingId, "Expense.Create", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه ثبت هزینه برای این ساختمان را ندارید.");
        }

        // گام ۲: ادامه منطق اصلی برنامه (بدون تغییر)
        var expense = new Expense
        {
            BuildingId = request.BuildingId,
            Title = request.Title,
            Description = request.Description,
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate.ToUniversalTime(),
            Category = request.Category,
            AllocationMethod = request.AllocationMethod,
            RecordedByUserId = request.RecordedByUserId
        };

        await _context.Expenses.AddAsync(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return expense.Id;
    }
}
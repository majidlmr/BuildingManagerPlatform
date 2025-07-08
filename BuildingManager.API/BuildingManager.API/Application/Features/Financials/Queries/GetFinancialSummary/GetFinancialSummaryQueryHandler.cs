using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Financials.Queries.GetFinancialSummary;

public class GetFinancialSummaryQueryHandler : IRequestHandler<GetFinancialSummaryQuery, FinancialSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public GetFinancialSummaryQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<FinancialSummaryDto> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        // ✅ بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز
        var canRead = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Financials.ReadSummary", cancellationToken);
        if (!canRead)
        {
            throw new ForbiddenAccessException("شما اجازه دسترسی به گزارش مالی این ساختمان را ندارید.");
        }

        var startDate = request.StartDate.ToUniversalTime();
        var endDate = request.EndDate.ToUniversalTime();

        var expenses = await _context.Expenses
            .Where(e => e.BuildingId == request.BuildingId && e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
            .Select(e => new TransactionDto(e.Title, "هزینه", -e.Amount, e.ExpenseDate, e.Category))
            .ToListAsync(cancellationToken);

        var revenues = await _context.Revenues
            .Where(r => r.BuildingId == request.BuildingId && r.RevenueDate >= startDate && r.RevenueDate <= endDate)
            .Select(r => new TransactionDto(r.Title, "درآمد", r.Amount, r.RevenueDate, r.Category))
            .ToListAsync(cancellationToken);

        var paidInvoices = await _context.Transactions
            .Where(t => t.Invoice.BuildingId == request.BuildingId && t.Status == "Succeeded" && t.PaidAt >= startDate && t.PaidAt <= endDate)
            .Select(t => new TransactionDto($"پرداخت شارژ: {t.Invoice.Description}", "درآمد", t.Amount, t.PaidAt, "شارژ"))
            .ToListAsync(cancellationToken);

        var allTransactions = expenses.Concat(revenues).Concat(paidInvoices).OrderByDescending(t => t.Date).ToList();

        var totalRevenue = allTransactions.Where(t => t.Type == "درآمد").Sum(t => t.Amount);
        var totalExpense = expenses.Sum(e => e.Amount);

        var summary = new FinancialSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalExpense = totalExpense,
            CurrentBalance = totalRevenue + totalExpense,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Transactions = allTransactions
        };

        return summary;
    }
}
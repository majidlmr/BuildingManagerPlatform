using BuildingManager.API.Domain.Enums;
using System;

namespace BuildingManager.API.Application.Features.Financials.Commands.CreateExpense;

public record CreateExpenseRequest(
    string Title,
    string? Description,
    decimal Amount,
    DateTime ExpenseDate,
    string Category,
    AllocationMethod AllocationMethod // فیلد جدید برای دریافت روش تقسیم هزینه
);
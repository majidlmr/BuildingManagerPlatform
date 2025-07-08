using BuildingManager.API.Domain.Enums;
using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Financials.Commands.CreateExpense;

public record CreateExpenseCommand(
    int BuildingId,
    string Title,
    string? Description,
    decimal Amount,
    DateTime ExpenseDate,
    string Category,
    AllocationMethod AllocationMethod, // فیلد جدید
    int RecordedByUserId
) : IRequest<int>;
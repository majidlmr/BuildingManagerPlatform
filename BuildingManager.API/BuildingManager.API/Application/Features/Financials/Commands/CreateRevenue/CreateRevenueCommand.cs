using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Financials.Commands.CreateRevenue;

public record CreateRevenueCommand(
    int BuildingId,
    string Title,
    string? Description,
    decimal Amount,
    DateTime RevenueDate,
    string Category,
    int RecordedByUserId
) : IRequest<int>;
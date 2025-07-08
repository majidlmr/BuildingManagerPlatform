using System;

namespace BuildingManager.API.Application.Features.Financials.Commands.CreateRevenue;

public record CreateRevenueRequest(
    string Title,
    string? Description,
    decimal Amount,
    DateTime RevenueDate,
    string Category
);
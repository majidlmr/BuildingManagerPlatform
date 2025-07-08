using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Financials.Queries.GetFinancialSummary;

public record GetFinancialSummaryQuery(
    int BuildingId,
    DateTime StartDate,
    DateTime EndDate,
    int RequestingUserId
) : IRequest<FinancialSummaryDto>;
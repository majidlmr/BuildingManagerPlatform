using MediatR;

namespace BuildingManager.API.Application.Features.Tickets.Queries.GetBuildingTickets;

// این Query شناسه ساختمان و کاربر درخواست دهنده را می‌گیرد
public record GetBuildingTicketsQuery(int BuildingId, int RequestingUserId) : IRequest<List<TicketSummaryDto>>;
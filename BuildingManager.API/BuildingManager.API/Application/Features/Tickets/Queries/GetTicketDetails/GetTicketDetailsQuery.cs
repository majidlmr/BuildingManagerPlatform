using MediatR;

namespace BuildingManager.API.Application.Features.Tickets.Queries.GetTicketDetails;

public record GetTicketDetailsQuery(Guid PublicId, int RequestingUserId) : IRequest<TicketDetailsDto>;
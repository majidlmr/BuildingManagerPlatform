namespace BuildingManager.API.Application.Features.Tickets.Queries.GetBuildingTickets;

public record TicketSummaryDto(
    Guid PublicId,
    string Title,
    string Status,
    string Priority,
    string Category,
    DateTime CreatedAt
);
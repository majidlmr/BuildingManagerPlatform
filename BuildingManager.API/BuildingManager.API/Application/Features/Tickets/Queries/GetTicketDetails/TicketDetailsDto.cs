namespace BuildingManager.API.Application.Features.Tickets.Queries.GetTicketDetails;

public record TicketDetailsDto(
    Guid PublicId,
    string Title,
    string Description,
    string Status,
    string Priority,
    string Category,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string ReportedBy, // نام کاربری که تیکت را ثبت کرده
    int? UnitId // شناسه واحد (در صورت وجود)
);
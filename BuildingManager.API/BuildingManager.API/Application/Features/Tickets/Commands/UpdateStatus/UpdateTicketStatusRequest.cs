namespace BuildingManager.API.Application.Features.Tickets.Commands.UpdateStatus;

// مدلی که از کلاینت دریافت می‌شود
public record UpdateTicketStatusRequest(string NewStatus);
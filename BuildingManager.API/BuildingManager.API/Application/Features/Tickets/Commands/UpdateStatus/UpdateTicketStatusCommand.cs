using MediatR;

namespace BuildingManager.API.Application.Features.Tickets.Commands.UpdateStatus;

public record UpdateTicketStatusCommand(
    Guid PublicId,      // شناسه تیکتی که باید آپدیت شود
    string NewStatus,   // وضعیت جدید
    int RequestingUserId // شناسه کاربری که درخواست را داده (برای احراز هویت)
) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند
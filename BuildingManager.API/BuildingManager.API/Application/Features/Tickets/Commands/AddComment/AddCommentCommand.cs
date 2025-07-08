using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Tickets.Commands.AddComment;

public record AddCommentCommand(
    Guid TicketPublicId, // شناسه تیکتی که کامنت برای آن ثبت می‌شود
    string Comment,      // متن کامنت
    int UserId           // شناسه کاربری که کامنت را ثبت کرده
) : IRequest<int>; // شناسه کامنت جدید را برمی‌گرداند
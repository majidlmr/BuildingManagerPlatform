// File: Application/Features/Chat/Commands/DeleteMessage/DeleteMessageCommand.cs
using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Chat.Commands.DeleteMessage;

/// <summary>
/// دستوری برای حذف یک پیام توسط ارسال‌کننده آن.
/// </summary>
public record DeleteMessageCommand(
    Guid MessagePublicId, // شناسه عمومی پیامی که باید حذف شود
    int RequestingUserId  // شناسه کاربری که درخواست حذف را داده است
) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند
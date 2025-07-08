// File: Application/Features/Chat/Commands/StartConversation/StartConversationCommand.cs
using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Chat.Commands.StartConversation;

/// <summary>
/// دستوری برای شروع یک گفتگوی دو نفره جدید.
/// این دستور شناسه کاربری که گفتگو را شروع می‌کند (مدیر) و شناسه کاربری که
/// به گفتگو دعوت می‌شود (ساکن) را دریافت می‌کند.
/// </summary>
public record StartConversationCommand(
    int InitiatorUserId, // شناسه کاربری که چت را شروع می‌کند (مدیر)
    int OtherUserId      // شناسه کاربر طرف مقابل (ساکن)
) : IRequest<Guid>; // شناسه عمومی (PublicId) گفتگوی ایجاد شده یا موجود را برمی‌گرداند
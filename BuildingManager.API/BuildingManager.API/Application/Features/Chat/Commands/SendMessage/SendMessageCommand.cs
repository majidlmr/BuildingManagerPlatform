// File: Application/Features/Chat/Commands/SendMessage/SendMessageCommand.cs
using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Chat.Commands.SendMessage;

/// <summary>
/// دستوری برای ارسال یک پیام در یک گفتگوی مشخص.
/// </summary>
public record SendMessageCommand(
    Guid ConversationPublicId,
    int SenderUserId,
    string Content,
    bool IsAnonymous
) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند
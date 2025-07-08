// File: Application/Features/Chat/Queries/GetConversationMessages/GetConversationMessagesQuery.cs
using MediatR;
using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Chat.Queries.GetConversationMessages;

/// <summary>
/// دستوری برای دریافت تاریخچه پیام‌های یک گفتگوی مشخص.
/// </summary>
public record GetConversationMessagesQuery(
    Guid ConversationPublicId,
    int RequestingUserId
) : IRequest<List<MessageDto>>;
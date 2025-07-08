// File: Application/Features/Chat/Queries/GetMyConversations/GetMyConversationsQuery.cs
using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Chat.Queries.GetMyConversations;

/// <summary>
/// دستوری برای دریافت لیست تمام گفتگوهای یک کاربر.
/// </summary>
public record GetMyConversationsQuery(int UserId) : IRequest<List<ConversationDto>>;
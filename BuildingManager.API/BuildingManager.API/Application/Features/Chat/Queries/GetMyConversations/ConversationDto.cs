// File: Application/Features/Chat/Queries/GetMyConversations/ConversationDto.cs
using System;

namespace BuildingManager.API.Application.Features.Chat.Queries.GetMyConversations;

/// <summary>
/// مدل اصلی برای نمایش یک گفتگو در لیست گفتگوهای کاربر.
/// </summary>
public record ConversationDto
{
    public Guid PublicId { get; set; }
    public string ConversationName { get; set; }
    public string? ConversationImageUrl { get; set; }
    public LastMessageDto? LastMessage { get; set; }
}

/// <summary>
/// مدلی برای نمایش خلاصه آخرین پیام یک گفتگو.
/// </summary>
public record LastMessageDto(
    string Content,
    DateTime SentAt,
    string SentBy
);
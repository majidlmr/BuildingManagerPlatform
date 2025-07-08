// File: Application/Features/Chat/Queries/GetConversationMessages/MessageDto.cs
using System;

namespace BuildingManager.API.Application.Features.Chat.Queries.GetConversationMessages;

/// <summary>
/// مدلی برای نمایش یک پیام در تاریخچه گفتگو.
/// </summary>
public record MessageDto(
    Guid MessagePublicId, // برای قابلیت‌هایی مانند پاسخ دادن یا حذف پیام
    string Content,
    DateTime SentAt,
    string SenderName,
    bool IsSentByCurrentUser, // برای نمایش پیام در سمت راست یا چپ صفحه چت
    bool IsDeleted // برای نمایش "این پیام حذف شده است"
);
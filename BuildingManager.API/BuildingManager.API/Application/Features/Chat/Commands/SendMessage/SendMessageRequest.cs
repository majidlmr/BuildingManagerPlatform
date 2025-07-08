// File: Application/Features/Chat/Commands/SendMessage/SendMessageRequest.cs
namespace BuildingManager.API.Application.Features.Chat.Commands.SendMessage;

/// <summary>
/// مدل ورودی برای دریافت اطلاعات یک پیام جدید از کلاینت (API).
/// </summary>
public record SendMessageRequest(
    string Content,
    bool IsAnonymous = false
);
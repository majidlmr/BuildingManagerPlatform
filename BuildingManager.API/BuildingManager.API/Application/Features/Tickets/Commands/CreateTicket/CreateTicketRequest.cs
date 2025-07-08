// File: Application/Features/Tickets/Commands/CreateTicket/CreateTicketRequest.cs
namespace BuildingManager.API.Application.Features.Tickets.Commands.CreateTicket;

/// <summary>
/// مدل ورودی برای دریافت اطلاعات یک تیکت جدید از کلاینت (API).
/// </summary>
public record CreateTicketRequest(
    string Title,
    string Description,
    string Category,
    string Priority,
    int? UnitId,
    string? AttachmentUrl,

    // 🚀 فیلد جدید: این فیلد از ورودی API خوانده می‌شود تا مشخص کند
    // آیا کاربر درخواست ارسال تیکت به صورت ناشناس را دارد یا خیر.
    bool IsAnonymous
);
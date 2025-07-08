// File: Application/Features/Tickets/Commands/CreateTicket/CreateTicketCommand.cs
using MediatR;
using System; // Guid به این using نیاز دارد

/// <summary>
/// دستوری برای ایجاد یک تیکت جدید.
/// این دستور تمام اطلاعات لازم برای ساخت یک موجودیت تیکت را حمل می‌کند.
/// </summary>
public record CreateTicketCommand(
    int BuildingId,
    int? UnitId,
    int ReportedByUserId,
    string Title,
    string Description,
    string Category,
    string Priority,
    string? AttachmentUrl,

    // 🚀 پارامتر جدید: برای پیاده‌سازی قابلیت ارسال ناشناس
    // مقدار این فیلد توسط کاربر از طریق کلاینت (مثلاً یک چک‌باکس) مشخص می‌شود.
    bool IsAnonymous

) : IRequest<Guid>; // این دستور شناسه عمومی تیکت جدید را برمی‌گرداند
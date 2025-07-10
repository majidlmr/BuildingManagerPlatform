// File: Application/Features/Tickets/Commands/CreateTicket/CreateTicketCommand.cs
using MediatR;
using System; // Guid به این using نیاز دارد
using BuildingManager.API.Domain.Enums; // Added for Enums

// Namespace should be defined if not present globally for the record
namespace BuildingManager.API.Application.Features.Tickets.Commands.CreateTicket
{
    /// <summary>
    /// دستوری برای ایجاد یک تیکت جدید.
    /// این دستور تمام اطلاعات لازم برای ساخت یک موجودیت تیکت را حمل می‌کند.
    /// </summary>
    public record CreateTicketCommand(
        int BlockId, // Changed from BuildingId
        int? UnitId,
        int ReportedByUserId,
        // int? ComplexId, // Optional: if a ticket can be directly for a complex or to override block's complex
        string Title,
        string Description,
        TicketCategory Category, // Changed to Enum
        TicketPriority Priority, // Changed to Enum
        // string? AttachmentUrl, // Removed - Attachments handled separately
        bool IsAnonymous
    ) : IRequest<Guid>; // این دستور شناسه عمومی تیکت جدید را برمی‌گرداند
}
using BuildingManager.API.Domain.Common;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از ثبت یک تیکت از نوع "گزارش پارک اشتباه" منتشر می‌شود.
/// </summary>
public class ParkingComplaintTicketCreatedEvent : DomainEvent
{
    public int TicketId { get; }
    public string AttachmentUrl { get; }

    public ParkingComplaintTicketCreatedEvent(int ticketId, string attachmentUrl)
    {
        TicketId = ticketId;
        AttachmentUrl = attachmentUrl;
    }
}
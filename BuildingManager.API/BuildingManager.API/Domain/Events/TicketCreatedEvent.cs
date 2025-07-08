using BuildingManager.API.Domain.Common;
using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از ایجاد موفقیت‌آمیز هر نوع تیکت جدید در سیستم، منتشر می‌شود.
/// </summary>
public class TicketCreatedEvent : DomainEvent
{
    public Ticket Ticket { get; }

    public TicketCreatedEvent(Ticket ticket)
    {
        Ticket = ticket;
    }
}
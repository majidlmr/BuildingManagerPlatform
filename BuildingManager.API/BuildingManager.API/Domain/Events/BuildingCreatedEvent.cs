using BuildingManager.API.Domain.Common;
using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از ایجاد موفقیت‌آمیز یک ساختمان جدید در سیستم، منتشر می‌شود.
/// </summary>
public class BuildingCreatedEvent : DomainEvent
{
    public Building Building { get; }

    public BuildingCreatedEvent(Building building)
    {
        Building = building;
    }
}
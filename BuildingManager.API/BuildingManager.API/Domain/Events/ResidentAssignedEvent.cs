using BuildingManager.API.Domain.Common;
using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از تخصیص موفقیت‌آمیز یک ساکن به یک واحد منتشر می‌شود.
/// این رویداد شامل اطلاعات کلیدی برای انجام عملیات‌های بعدی است.
/// </summary>
public class ResidentAssignedEvent : DomainEvent
{
    public int UserId { get; }
    public int BuildingId { get; }

    public ResidentAssignedEvent(int userId, int buildingId)
    {
        UserId = userId;
        BuildingId = buildingId;
    }
}
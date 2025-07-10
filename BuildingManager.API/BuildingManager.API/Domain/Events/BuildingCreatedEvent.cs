using BuildingManager.API.Domain.Common;
using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از ایجاد موفقیت‌آمیز یک بلوک جدید در سیستم، منتشر می‌شود.
/// </summary>
public class BuildingCreatedEvent : DomainEvent
{
    public Block Block { get; } // Changed from Building to Block

    public BuildingCreatedEvent(Block block) // Changed from Building to Block
    {
        Block = block;
    }
}
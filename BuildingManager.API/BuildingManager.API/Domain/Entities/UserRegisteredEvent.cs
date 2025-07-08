using BuildingManager.API.Domain.Common;
using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Events;

/// <summary>
/// رویدادی که پس از ثبت‌نام موفقیت‌آمیز یک کاربر جدید در سیستم، منتشر می‌شود.
/// این رویداد شامل اطلاعات کاربر جدید است.
/// </summary>
public class UserRegisteredEvent : DomainEvent
{
    public User User { get; }

    public UserRegisteredEvent(User user)
    {
        User = user;
    }
}
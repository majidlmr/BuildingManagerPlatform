using MediatR;
using System;

namespace BuildingManager.API.Domain.Common;

/// <summary>
/// یک کلاس پایه انتزاعی برای تمام رویدادهای دامنه (Domain Events) در برنامه.
/// این کلاس از INotification ارث‌بری می‌کند تا توسط MediatR قابل شناسایی و انتشار باشد.
/// </summary>
public abstract class DomainEvent : INotification
{
    /// <summary>
    /// تاریخ و زمان وقوع رویداد.
    /// </summary>
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
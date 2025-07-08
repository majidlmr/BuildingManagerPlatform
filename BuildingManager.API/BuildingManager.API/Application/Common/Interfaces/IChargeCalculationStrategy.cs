using BuildingManager.API.Domain.Entities;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Common.Interfaces;

/// <summary>
/// اینترفیس برای الگوی استراتژی محاسبه شارژ.
/// هر کلاس که این اینترفیس را پیاده‌سازی کند، یک روش متفاوت برای محاسبه
/// و توزیع هزینه‌ها بین واحدهای یک ساختمان را ارائه می‌دهد.
/// </summary>
public interface IChargeCalculationStrategy
{
    /// <summary>
    /// نام منحصر به فرد استراتژی که در دیتابیس ذخیره می‌شود.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// هزینه‌های ساختمان را بین واحدهای آن توزیع کرده و آیتم‌های صورتحساب را برمی‌گرداند.
    /// </summary>
    /// <param name="expenses">لیست تمام هزینه‌های دوره.</param>
    /// <param name="units">لیست تمام واحدهای ساختمان.</param>
    /// <returns>یک دیکشنری که کلید آن شناسه واحد (UnitId) و مقدار آن لیست آیتم‌های صورتحساب برای آن واحد است.</returns>
    Dictionary<int, List<InvoiceItem>> CalculateDues(IEnumerable<Expense> expenses, IEnumerable<Unit> units);
}
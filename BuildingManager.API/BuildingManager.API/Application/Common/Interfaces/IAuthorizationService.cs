// File: Application/Common/Interfaces/IAuthorizationService.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Common.Interfaces;

/// <summary>
/// سرویس متمرکز برای بررسی دسترسی‌ها بر اساس سیستم RBAC.
/// این اینترفیس هم متدهای سطح بالا برای سناریوهای رایج و هم یک متد اصلی
/// برای بررسی دسترسی‌های دانه‌ریز را فراهم می‌کند.
/// </summary>
public interface IAuthorizationService
{
    // --- متد اصلی و قدرتمند RBAC ---

    /// <summary>
    /// بررسی می‌کند که آیا یک کاربر، دسترسی (Permission) خاصی را در یک ساختمان مشخص دارد یا خیر.
    /// این متد هسته اصلی سیستم دسترسی است.
    /// </summary>
    /// <param name="userId">شناسه کاربری که باید بررسی شود.</param>
    /// <param name="buildingId">شناسه ساختمانی که دسترسی در آن مورد نیاز است.</param>
    /// <param name="permissionName">نام دسترسی مورد نیاز (مثلا: "Ticket.Update").</param>
    /// <param name="cancellationToken">توکن انصراف.</param>
    /// <returns>True اگر کاربر دسترسی را داشته باشد، در غیر این صورت False.</returns>
    Task<bool> HasPermissionAsync(int userId, int buildingId, string permissionName, CancellationToken cancellationToken = default);

    // --- متدهای کمکی و کاربردی (ترکیبی از نسخه شما و معماری جدید) ---

    /// <summary>
    /// بررسی می‌کند که آیا کاربر عضو یک ساختمان (با هر نقشی) است یا خیر.
    /// این متد برای دسترسی‌های عمومی مانند مشاهده اطلاعات کلی ساختمان استفاده می‌شود.
    /// </summary>
    Task<bool> IsMemberOfBuildingAsync(int userId, int buildingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی می‌کند که آیا کاربر به یک تیکت خاص دسترسی دارد یا خیر.
    /// یک کاربر در صورتی به تیکت دسترسی دارد که:
    /// 1. خودش گزارش‌دهنده تیکت باشد.
    /// 2. دسترسی "Ticket.Read" را در ساختمان مربوطه داشته باشد.
    /// </summary>
    Task<bool> CanAccessTicketAsync(int userId, Guid ticketPublicId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی می‌کند که آیا کاربر به یک صورتحساب خاص دسترسی دارد یا خیر.
    /// یک کاربر در صورتی به صورتحساب دسترسی دارد که:
    /// 1. صورتحساب برای خود او صادر شده باشد.
    /// 2. دسترسی "Billing.Read" را در ساختمان مربوطه داشته باشد.
    /// </summary>
    Task<bool> CanAccessInvoiceAsync(int userId, Guid invoicePublicId, CancellationToken cancellationToken = default);
}
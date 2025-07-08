// File: Infrastructure/Services/AuthorizationService.cs
using BuildingManager.API.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس کنترل دسترسی با استفاده از مدل جدید RBAC.
/// این کلاس منطق‌های پیچیده دسترسی را کپسوله کرده و متدهای ساده‌ای را به لایه Application ارائه می‌دهد.
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IApplicationDbContext _context;

    public AuthorizationService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(int userId, int buildingId, string permissionName, CancellationToken cancellationToken = default)
    {
        // با یک کوئری بهینه، بررسی می‌کنیم که آیا کاربر نقشی در این ساختمان دارد
        // که آن نقش، دسترسی مورد نظر را داشته باشد.
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.Role.BuildingId == buildingId)
            .AnyAsync(ur => ur.Role.Permissions.Any(rp => rp.Permission.Name == permissionName), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsMemberOfBuildingAsync(int userId, int buildingId, CancellationToken cancellationToken = default)
    {
        // با یک کوئری بهینه، بررسی می‌کنیم که آیا کاربر حداقل یک نقش در این ساختمان دارد یا خیر.
        // این شامل مدیران و ساکنین (که نقشی به آن‌ها تخصیص داده شده) می‌شود.
        return await _context.UserRoles
            .AsNoTracking()
            .AnyAsync(ur => ur.UserId == userId && ur.Role.BuildingId == buildingId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> CanAccessTicketAsync(int userId, Guid ticketPublicId, CancellationToken cancellationToken)
    {
        var ticketInfo = await _context.Tickets
            .AsNoTracking()
            .Where(t => t.PublicId == ticketPublicId)
            .Select(t => new { t.BuildingId, t.ReportedByUserId })
            .FirstOrDefaultAsync(cancellationToken);

        if (ticketInfo == null) return false;

        // کاربر همیشه به تیکت‌های خودش دسترسی دارد.
        if (ticketInfo.ReportedByUserId == userId) return true;

        // در غیر این صورت، بررسی می‌کنیم که آیا دسترسی خواندن تیکت را دارد یا خیر.
        // این دسترسی می‌تواند به نقش‌هایی مانند "مدیر"، "عضو هیئت مدیره" یا "پشتیبان فنی" داده شود.
        return await HasPermissionAsync(userId, ticketInfo.BuildingId, "Ticket.Read", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> CanAccessInvoiceAsync(int userId, Guid invoicePublicId, CancellationToken cancellationToken)
    {
        var invoiceInfo = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.PublicId == invoicePublicId)
            .Select(i => new { i.UserId, i.BuildingId })
            .FirstOrDefaultAsync(cancellationToken);

        if (invoiceInfo == null) return false;

        // کاربر همیشه به صورتحساب‌های خودش دسترسی دارد.
        if (invoiceInfo.UserId == userId) return true;

        // در غیر این صورت، بررسی می‌کنیم که آیا دسترسی مشاهده صورتحساب را دارد یا خیر.
        // این دسترسی می‌تواند به نقش‌هایی مانند "مدیر" یا "حسابدار" داده شود.
        return await HasPermissionAsync(userId, invoiceInfo.BuildingId, "Billing.Read", cancellationToken);
    }
}
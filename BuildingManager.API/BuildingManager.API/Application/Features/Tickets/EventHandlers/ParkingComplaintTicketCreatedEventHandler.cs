using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.EventHandlers;

/// <summary>
/// این Handler به رویداد عمومی TicketCreatedEvent گوش می‌دهد و فقط در صورتی که
/// دسته تیکت "گزارش پارک اشتباه" باشد، منطق OCR برای خواندن پلاک را اجرا می‌کند.
/// </summary>
public class ParkingComplaintTicketCreatedEventHandler : INotificationHandler<TicketCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly IOcrService _ocrService;
    private readonly IUnitOfWork _unitOfWork;

    public ParkingComplaintTicketCreatedEventHandler(IApplicationDbContext context, IOcrService ocrService, IUnitOfWork unitOfWork)
    {
        _context = context;
        _ocrService = ocrService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// منطق اصلی را پس از دریافت رویداد اجرا می‌کند.
    /// </summary>
    public async Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
    {
        var ticket = notification.Ticket;

        // گام ۱ (کلیدی): بررسی شرط قبل از اجرای منطق
        // اگر تیکت از نوع مورد نظر ما نیست یا پیوست ندارد، هیچ کاری انجام نده.
        if (!ticket.Category.Equals("گزارش پارک اشتباه", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(ticket.AttachmentUrl))
        {
            return;
        }

        // گام ۲: خواندن پلاک از روی عکس با استفاده از سرویس OCR
        var licensePlate = await _ocrService.ReadLicensePlateFromUrlAsync(ticket.AttachmentUrl);
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            await AddCommentToTicketAsync(ticket.Id, "سیستم قادر به تشخیص پلاک از روی تصویر نبود. لطفاً به صورت دستی بررسی فرمایید.", cancellationToken);
            return;
        }

        // گام ۳: جستجو در دیتابیس برای پیدا کردن خودرو با پلاک خوانده شده
        var vehicle = await _context.Vehicles
            .Include(v => v.User) // برای دسترسی به اطلاعات مالک خودرو
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate.ToUpper(), cancellationToken);

        if (vehicle != null)
        {
            // اگر خودرو پیدا شد، یک کامنت حاوی اطلاعات مالک ثبت کن
            var comment = $"پلاک '{licensePlate}' شناسایی شد. این خودرو متعلق به آقای/خانم {vehicle.User.FullName} است. پیام هشدار برای ایشان ارسال گردید.";
            await AddCommentToTicketAsync(ticket.Id, comment, cancellationToken);
        }
        else
        {
            // اگر خودرو پیدا نشد، این موضوع را در تیکت ثبت کن
            var comment = $"خودرویی با پلاک '{licensePlate}' در سیستم ثبت نشده است. موضوع جهت بررسی دستی به مدیر ارجاع داده شد.";
            await AddCommentToTicketAsync(ticket.Id, comment, cancellationToken);
        }
    }

    /// <summary>
    /// یک متد کمکی برای افزودن کامنت خودکار به تیکت.
    /// </summary>
    private async Task AddCommentToTicketAsync(int ticketId, string commentText, CancellationToken cancellationToken)
    {
        // در سیستم واقعی، یک کاربر سیستمی با شناسه مشخص تعریف می‌شود
        // یا این کامنت به نام مدیر ساختمان ثبت می‌شود.
        const int systemUserId = 0;

        var ticketUpdate = new TicketUpdate
        {
            TicketId = ticketId,
            Comment = commentText,
            UpdateByUserId = systemUserId, // کامنت از طرف سیستم ثبت می‌شود
            CreatedAt = DateTime.UtcNow
        };

        await _context.TicketUpdates.AddAsync(ticketUpdate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
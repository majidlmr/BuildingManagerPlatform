using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Events;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.Commands.CreateTicket;

/// <summary>
/// پردازشگر دستور ایجاد یک تیکت جدید.
/// این کلاس مسئولیت‌های زیر را بر عهده دارد:
/// 1. بررسی دسترسی کاربر برای ثبت تیکت.
/// 2. ایجاد موجودیت تیکت با اطلاعات دریافتی.
/// 3. ذخیره تیکت در دیتابیس.
/// 4. انتشار رویداد عمومی 'TicketCreatedEvent' برای اطلاع‌رسانی به سایر ماژول‌ها.
/// </summary>
public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;
    private readonly IAuthorizationService _authorizationService;

    public CreateTicketCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IPublisher publisher,
        IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _authorizationService = authorizationService;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی امنیتی - آیا کاربری که تیکت را ثبت می‌کند، عضو ساختمان است؟
        var canAccess = await _authorizationService.IsMemberOfBuildingAsync(request.ReportedByUserId, request.BuildingId, cancellationToken);
        if (!canAccess)
        {
            throw new ForbiddenAccessException("شما اجازه ثبت تیکت در این ساختمان را ندارید.");
        }

        // گام ۲: ایجاد موجودیت تیکت جدید با تمام اطلاعات ورودی
        var ticket = new Ticket
        {
            BuildingId = request.BuildingId,
            UnitId = request.UnitId,
            ReportedByUserId = request.ReportedByUserId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Priority = request.Priority,
            AttachmentUrl = request.AttachmentUrl,
            Status = "Open", // وضعیت اولیه تمام تیکت‌ها
            CreatedAt = DateTime.UtcNow,
            IsAnonymous = request.IsAnonymous
        };

        // گام ۳: افزودن تیکت به دیتابیس و ذخیره تغییرات
        await _context.Tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // گام ۴ (مهم): انتشار یک رویداد عمومی پس از ساخت هر نوع تیکت
        // این کار به سایر بخش‌های سیستم اجازه می‌دهد به این اتفاق واکنش نشان دهند.
        await _publisher.Publish(new TicketCreatedEvent(ticket), cancellationToken);

        // گام ۵: برگرداندن شناسه عمومی تیکت برای استفاده کلاینت
        return ticket.PublicId;
    }
}
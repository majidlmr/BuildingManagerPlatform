// File: Application/Features/Tickets/Queries/GetTicketDetails/GetTicketDetailsQueryHandler.cs
using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.Queries.GetTicketDetails;

public class GetTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, TicketDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public GetTicketDetailsQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<TicketDetailsDto> Handle(GetTicketDetailsQuery request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی امنیتی - آیا کاربر اجازه دسترسی به این تیکت را دارد؟
        var canAccess = await _authorizationService.CanAccessTicketAsync(request.RequestingUserId, request.PublicId, cancellationToken);
        if (!canAccess)
        {
            throw new ForbiddenAccessException("شما اجازه مشاهده این تیکت را ندارید.");
        }

        // گام ۲: واکشی اطلاعات تیکت از پایگاه داده
        var ticket = await _context.Tickets
            .AsNoTracking()
            .Include(t => t.ReportedBy) // برای دسترسی به اطلاعات کاربر گزارش‌دهنده
            .FirstOrDefaultAsync(t => t.PublicId == request.PublicId, cancellationToken);

        if (ticket == null)
        {
            // استفاده از خطای سفارشی برای مدیریت پاسخ 404
            throw new NotFoundException(nameof(Ticket), request.PublicId);
        }

        // 🚀 تغییر اصلی: بررسی وضعیت ناشناس بودن تیکت
        // اگر تیکت ناشناس باشد، نام ارسال‌کننده را "کاربر ناشناس" قرار می‌دهیم.
        // در غیر این صورت، نام کامل کاربر را نمایش می‌دهیم.
        string reportedByName = ticket.IsAnonymous ? "کاربر ناشناس" : ticket.ReportedBy.FullName;

        // گام ۳: ساخت DTO برای ارسال به کلاینت
        var ticketDetails = new TicketDetailsDto(
            ticket.PublicId,
            ticket.Title,
            ticket.Description,
            ticket.Status,
            ticket.Priority,
            ticket.Category,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            reportedByName, // استفاده از نام تعیین شده در مرحله قبل
            ticket.UnitId
        );

        return ticketDetails;
    }
}
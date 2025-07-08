// File: Application/Features/Tickets/Queries/GetBuildingTickets/GetBuildingTicketsQueryHandler.cs
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Tickets.Queries.GetBuildingTickets;

/// <summary>
/// پردازشگر دستور دریافت لیست تیکت‌های یک ساختمان.
/// این نسخه شامل بررسی کامل دسترسی کاربر است.
/// </summary>
public class GetBuildingTicketsQueryHandler : IRequestHandler<GetBuildingTicketsQuery, List<TicketSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authService;

    /// <summary>
    /// سازنده کلاس که سرویس‌های مورد نیاز را تزریق می‌کند.
    /// </summary>
    public GetBuildingTicketsQueryHandler(IApplicationDbContext context, IAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<List<TicketSummaryDto>> Handle(GetBuildingTicketsQuery request, CancellationToken cancellationToken)
    {
        // 🚀 مهم‌ترین تغییر: فراخوانی متد با نام جدید و صحیح
        // نام متد از IsUserMemberOfBuilding به IsMemberOfBuildingAsync تغییر کرده است.
        var canAccess = await _authService.IsMemberOfBuildingAsync(request.RequestingUserId, request.BuildingId, cancellationToken);

        if (!canAccess)
        {
            throw new Exception("You are not authorized to access tickets for this building.");
        }

        // بقیه منطق کد شما بدون تغییر باقی می‌ماند
        var tickets = await _context.Tickets
            .Where(t => t.BuildingId == request.BuildingId)
            .OrderByDescending(t => t.CreatedAt)
            .AsNoTracking()
            .Select(t => new TicketSummaryDto(
                t.PublicId,
                t.Title,
                t.Status,
                t.Priority,
                t.Category,
                t.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return tickets;
    }
}
using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Announcements.Queries.GetAnnouncements;

public class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, List<AnnouncementDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService; // 👈 سرویس دسترسی

    public GetAnnouncementsQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService; // 👈 مقداردهی
    }

    public async Task<List<AnnouncementDto>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        // ✅ TODO تکمیل شد: بررسی عضویت کاربر در ساختمان
        var isMember = await _authorizationService.IsMemberOfBuildingAsync(request.RequestingUserId, request.BuildingId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException("شما اجازه مشاهده اعلان‌های این ساختمان را ندارید.");
        }

        // ادامه منطق بدون تغییر
        var announcements = await _context.Announcements
            .Where(a => a.BuildingId == request.BuildingId && (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.CreatedAt)
            .Include(a => a.CreatedByUser)
            .Select(a => new AnnouncementDto(
                a.Id,
                a.Title,
                a.Content,
                a.CreatedByUser.FullName,
                a.CreatedAt,
                a.IsPinned
            ))
            .ToListAsync(cancellationToken);

        return announcements;
    }
}
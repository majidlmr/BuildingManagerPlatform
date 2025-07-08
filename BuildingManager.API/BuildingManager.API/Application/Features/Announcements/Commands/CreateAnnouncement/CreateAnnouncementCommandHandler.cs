using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Announcements.Commands.CreateAnnouncement;

public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public CreateAnnouncementCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<int> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        // ✅ بررسی دسترسی با استفاده از سیستم جدید مبتنی بر مجوز "Announcement.Create"
        var canCreate = await _authorizationService.HasPermissionAsync(request.CreatedByUserId, request.BuildingId, "Announcement.Create", cancellationToken);
        if (!canCreate)
        {
            throw new ForbiddenAccessException("شما اجازه ایجاد اعلان برای این ساختمان را ندارید.");
        }

        var announcement = new Announcement
        {
            BuildingId = request.BuildingId,
            Title = request.Title,
            Content = request.Content,
            ExpiresAt = request.ExpiresAt?.ToUniversalTime(),
            IsPinned = request.IsPinned,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = System.DateTime.UtcNow
        };

        await _context.Announcements.AddAsync(announcement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return announcement.Id;
    }
}
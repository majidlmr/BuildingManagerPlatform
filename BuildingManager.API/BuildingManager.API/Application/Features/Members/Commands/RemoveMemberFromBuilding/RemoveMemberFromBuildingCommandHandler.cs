using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Members.Commands.RemoveMemberFromBuilding;

public class RemoveMemberFromBuildingCommandHandler : IRequestHandler<RemoveMemberFromBuildingCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public RemoveMemberFromBuildingCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task Handle(RemoveMemberFromBuildingCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی دسترسی کاربر درخواست‌دهنده
        var canRemove = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Member.Remove", cancellationToken);
        if (!canRemove)
        {
            throw new ForbiddenAccessException("شما اجازه حذف اعضا از این ساختمان را ندارید.");
        }

        if (request.MemberUserId == request.RequestingUserId)
        {
            throw new ValidationException("شما نمی‌توانید خودتان را از ساختمان حذف کنید.");
        }

        // گام ۲: پیدا کردن و حذف تمام نقش‌های کاربر در ساختمان مشخص شده
        var userRolesInBuilding = await _context.UserRoles
            .Where(ur => ur.UserId == request.MemberUserId && ur.Role.BuildingId == request.BuildingId)
            .ToListAsync(cancellationToken);

        if (userRolesInBuilding.Any())
        {
            _context.UserRoles.RemoveRange(userRolesInBuilding);
        }

        // گام ۳: پیدا کردن و حذف تخصیص مدیریت کاربر (اگر مدیر بوده)
        var managerAssignment = await _context.ManagerAssignments
            .FirstOrDefaultAsync(m => m.UserId == request.MemberUserId && m.BuildingId == request.BuildingId, cancellationToken);

        if (managerAssignment != null)
        {
            _context.ManagerAssignments.Remove(managerAssignment);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
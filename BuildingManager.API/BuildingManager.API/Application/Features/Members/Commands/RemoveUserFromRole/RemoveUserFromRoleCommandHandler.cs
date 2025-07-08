using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Members.Commands.RemoveUserFromRole;

public class RemoveUserFromRoleCommandHandler : IRequestHandler<RemoveUserFromRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public RemoveUserFromRoleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task Handle(RemoveUserFromRoleCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی دسترسی کاربر درخواست‌دهنده
        var canRemove = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Member.Remove", cancellationToken);
        if (!canRemove)
        {
            throw new ForbiddenAccessException("شما اجازه حذف نقش از کاربران این ساختمان را ندارید.");
        }

        // گام ۲: پیدا کردن رابطه نقش-کاربر مورد نظر
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == request.UserIdToRemove && ur.RoleId == request.RoleId, cancellationToken);

        if (userRole == null)
        {
            // اگر تخصیص از قبل وجود نداشته باشد، عملیات با موفقیت (بدون تغییر) پایان می‌یابد
            return;
        }

        // گام ۳: حذف رکورد از جدول واسط و ذخیره تغییرات
        _context.UserRoles.Remove(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
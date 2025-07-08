using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Roles.Commands.AssignPermissionToRole;

public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public AssignPermissionToRoleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        // گام ۱: بررسی دسترسی کاربر برای تخصیص دسترسی
        var canAssign = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Role.Assign", cancellationToken);
        if (!canAssign)
        {
            throw new ForbiddenAccessException("شما اجازه تخصیص دسترسی به نقش‌ها در این ساختمان را ندارید.");
        }

        // گام ۲: پیدا کردن نقش مورد نظر به همراه دسترسی‌های فعلی آن
        var role = await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == request.RoleId && r.BuildingId == request.BuildingId, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException("نقش مورد نظر در این ساختمان یافت نشد.");
        }

        // گام ۳: پیدا کردن دسترسی‌های معتبر بر اساس شناسه‌های دریافتی
        var permissionsToAssign = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (permissionsToAssign.Count != request.PermissionIds.Count)
        {
            throw new NotFoundException("یک یا چند مورد از دسترسی‌های ارسالی، نامعتبر است.");
        }

        // گام ۴: افزودن دسترسی‌های جدید به نقش (با جلوگیری از افزودن موارد تکراری)
        var currentPermissionIds = role.Permissions.Select(rp => rp.PermissionId).ToHashSet();

        foreach (var permission in permissionsToAssign)
        {
            if (!currentPermissionIds.Contains(permission.Id))
            {
                role.Permissions.Add(new RolePermission { PermissionId = permission.Id });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
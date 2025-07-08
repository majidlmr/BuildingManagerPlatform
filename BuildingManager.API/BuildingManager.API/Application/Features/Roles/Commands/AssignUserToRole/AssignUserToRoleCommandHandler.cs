using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Roles.Commands.AssignUserToRole;

public class AssignUserToRoleCommandHandler : IRequestHandler<AssignUserToRoleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public AssignUserToRoleCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task Handle(AssignUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var canAssign = await _authorizationService.HasPermissionAsync(request.RequestingUserId, request.BuildingId, "Role.Assign", cancellationToken);
        if (!canAssign)
        {
            throw new ForbiddenAccessException("شما اجازه تخصیص نقش به کاربران را ندارید.");
        }

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId && r.BuildingId == request.BuildingId, cancellationToken);
        if (!roleExists) throw new NotFoundException("نقش مورد نظر در این ساختمان یافت نشد.");

        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserIdToAssign, cancellationToken);
        if (!userExists) throw new NotFoundException("کاربر مورد نظر یافت نشد.");

        var alreadyAssigned = await _context.UserRoles.AnyAsync(ur => ur.RoleId == request.RoleId && ur.UserId == request.UserIdToAssign, cancellationToken);
        if (alreadyAssigned) return; // اگر از قبل تخصیص داده شده، عملیات جدیدی لازم نیست

        var userRole = new UserRole { RoleId = request.RoleId, UserId = request.UserIdToAssign };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
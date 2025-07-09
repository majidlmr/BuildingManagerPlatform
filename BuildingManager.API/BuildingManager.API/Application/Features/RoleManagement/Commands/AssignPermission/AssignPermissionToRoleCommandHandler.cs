using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.RoleManagement.Commands.AssignPermission
{
    public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        // private readonly ICurrentUserService _currentUserService;
        // private readonly IAuthorizationService _authzService;


        public AssignPermissionToRoleCommandHandler(IApplicationDbContext context /*, ICurrentUserService currentUserService, IAuthorizationService authzService*/)
        {
            _context = context;
            // _currentUserService = currentUserService;
            // _authzService = authzService;
        }

        public async Task<bool> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == request.RoleNormalizedName && !r.IsDeleted, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException(nameof(Role), request.RoleNormalizedName);
            }

            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == request.PermissionName && !p.IsDeleted, cancellationToken);
            if (permission == null)
            {
                throw new NotFoundException(nameof(Permission), request.PermissionName);
            }

            // TODO: Authorization check: Ensure the current user has permission to assign permissions to this role.
            // e.g., await _authzService.HasPermissionAsync(_currentUserService.UserId, "Permissions.Role.AssignPermissions");
            // Or more granularly, can they edit *this specific role*?

            var existingRolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id && !rp.IsDeleted, cancellationToken);

            if (existingRolePermission != null)
            {
                // Permission is already assigned to the role and not deleted.
                return true; // Or indicate "already assigned"
            }

            var newRolePermission = new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = request.AssignedByUserId // Should be current authenticated user's ID
            };

            await _context.RolePermissions.AddAsync(newRolePermission, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

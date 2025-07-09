using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingManager.API.Application.Common.Exceptions; // For NotFoundException

namespace BuildingManager.API.Application.Features.RoleManagement.Queries.GetRolePermissions
{
    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, List<PermissionResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetRolePermissionsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermissionResponseDto>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.NormalizedName == request.RoleNormalizedName && !r.IsDeleted, cancellationToken);

            if (role == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Role), request.RoleNormalizedName);
            }

            var permissions = await _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == role.Id && !rp.IsDeleted)
                .Include(rp => rp.Permission)
                .Where(rp => rp.Permission != null && !rp.Permission.IsDeleted) // Ensure permission itself is not deleted
                .Select(rp => new PermissionResponseDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Module = rp.Permission.Module,
                    Description = rp.Permission.Description
                })
                .ToListAsync(cancellationToken);

            return permissions;
        }
    }
}

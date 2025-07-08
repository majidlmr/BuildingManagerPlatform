using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Roles.Queries.GetBuildingRoles;

public class GetBuildingRolesQueryHandler : IRequestHandler<GetBuildingRolesQuery, List<RoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;

    public GetBuildingRolesQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<List<RoleDto>> Handle(GetBuildingRolesQuery request, CancellationToken cancellationToken)
    {
        var isMember = await _authorizationService.IsMemberOfBuildingAsync(request.RequestingUserId, request.BuildingId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException("شما اجازه مشاهده نقش‌های این ساختمان را ندارید.");
        }

        var roles = await _context.Roles
            .Where(r => r.BuildingId == request.BuildingId)
            .Include(r => r.Permissions)
            .ThenInclude(rp => rp.Permission)
            .Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.Permissions.Select(p => p.Permission.Name).ToList()
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return roles;
    }
}
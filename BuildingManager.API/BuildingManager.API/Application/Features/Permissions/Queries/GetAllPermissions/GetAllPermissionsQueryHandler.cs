using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPermissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionDto>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        // تمام دسترسی‌های موجود را از دیتابیس خوانده و به DTO تبدیل می‌کند
        var permissions = await _context.Permissions
            .AsNoTracking()
            .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return permissions;
    }
}
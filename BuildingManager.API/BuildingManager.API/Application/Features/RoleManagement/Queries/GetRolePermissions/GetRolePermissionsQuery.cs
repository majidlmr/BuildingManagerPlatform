using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.RoleManagement.Queries.GetRolePermissions
{
    public class GetRolePermissionsQuery : IRequest<List<PermissionResponseDto>>
    {
        public string RoleNormalizedName { get; set; }
    }

    public class PermissionResponseDto
    {
        public string Name { get; set; }
        public string Module { get; set; }
        public string? Description { get; set; }
    }
}

using MediatR;

namespace BuildingManager.API.Application.Features.RoleManagement.Commands.AssignPermission
{
    public class AssignPermissionToRoleCommand : IRequest<bool>
    {
        public string RoleNormalizedName { get; set; }
        public string PermissionName { get; set; }
        public int? AssignedByUserId { get; set; } // Optional: User performing the action
    }
}

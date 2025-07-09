using MediatR;
using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.UserManagement.Queries.GetUserRoles
{
    public class GetUserRolesQuery : IRequest<List<UserRoleResponseDto>>
    {
        public Guid UserPublicId { get; set; }
        // Optional: to filter roles for a specific context (e.g., roles within a specific complex or block)
        public Guid? TargetEntityPublicId { get; set; }
    }

    public class UserRoleResponseDto
    {
        public string RoleName { get; set; }
        public string RoleNormalizedName { get; set; }
        public string RoleDescription { get; set; }
        public Domain.Entities.HierarchyLevel RoleScope { get; set; } // System, Complex, Block
        public Guid? TargetEntityPublicId { get; set; } // PublicId of Complex or Block if role is scoped
        public string TargetEntityName { get; set; } // Name of the Complex or Block
        public Domain.Entities.AssignmentStatus AssignmentStatus { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}

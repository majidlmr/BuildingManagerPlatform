using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For AssignmentStatus

namespace BuildingManager.API.Application.Features.UserManagement.Commands.AssignRole
{
    public class AssignRoleToUserCommand : IRequest<bool>
    {
        public Guid UserPublicId { get; set; }
        public string RoleNormalizedName { get; set; } // Use NormalizedName for finding role
        public Guid? TargetEntityPublicId { get; set; } // PublicId of Complex or Block if role is scoped
        public AssignmentStatus InitialAssignmentStatus { get; set; } = AssignmentStatus.Active; // Default to Active
        public string? VerificationNotes { get; set; }
        public int? AssignedByUserId { get; set; } // ID of the user performing the assignment (e.g., an admin)
    }
}

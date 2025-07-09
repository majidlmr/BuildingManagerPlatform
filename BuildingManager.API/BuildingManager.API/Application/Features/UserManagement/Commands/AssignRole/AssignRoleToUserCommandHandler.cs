using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.UserManagement.Commands.AssignRole
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IAuthorizationService _authzService; // To check if assigner has permission

        public AssignRoleToUserCommandHandler(IApplicationDbContext context, IAuthorizationService authzService)
        {
            _context = context;
            _authzService = authzService;
        }

        public async Task<bool> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var userToAssign = await _context.Users.FirstOrDefaultAsync(u => u.PublicId == request.UserPublicId && !u.IsDeleted, cancellationToken);
            if (userToAssign == null)
            {
                throw new NotFoundException(nameof(User), request.UserPublicId);
            }

            var roleToAssign = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == request.RoleNormalizedName && !r.IsDeleted, cancellationToken);
            if (roleToAssign == null)
            {
                throw new NotFoundException(nameof(Role), request.RoleNormalizedName);
            }

            int? targetEntityId = null;
            if (request.TargetEntityPublicId.HasValue)
            {
                if (roleToAssign.AppliesToHierarchyLevel == HierarchyLevel.Complex)
                {
                    var complex = await _context.Complexes.FirstOrDefaultAsync(c => c.PublicId == request.TargetEntityPublicId.Value && !c.IsDeleted, cancellationToken);
                    if (complex == null) throw new NotFoundException("Complex", request.TargetEntityPublicId.Value);
                    targetEntityId = complex.Id;
                }
                else if (roleToAssign.AppliesToHierarchyLevel == HierarchyLevel.Block)
                {
                    var block = await _context.Blocks.FirstOrDefaultAsync(b => b.PublicId == request.TargetEntityPublicId.Value && !b.IsDeleted, cancellationToken);
                    if (block == null) throw new NotFoundException("Block", request.TargetEntityPublicId.Value);
                    targetEntityId = block.Id;
                }
            }
            else if (roleToAssign.AppliesToHierarchyLevel != HierarchyLevel.System)
            {
                // Roles scoped to Complex or Block require a TargetEntityId
                throw new ApplicationException($"Role '{roleToAssign.Name}' requires a target entity (Complex or Block).");
            }

            // TODO: Authorization Check: Ensure the request.AssignedByUserId (current authenticated user)
            // has permission to assign this role in this scope.
            // Example: await _authzService.HasPermissionAsync(request.AssignedByUserId, targetEntityId (if applicable for scope), "Permissions.User.AssignRoles");
            // This check needs to be more sophisticated based on who can assign what roles.
            // For MVP, SuperAdmin might be the only one allowed to call this initially.

            // Check if the assignment already exists (and is not soft-deleted)
            var existingAssignment = await _context.UserRoleAssignments
                .FirstOrDefaultAsync(ura => ura.UserId == userToAssign.Id &&
                                            ura.RoleId == roleToAssign.Id &&
                                            ura.TargetEntityId == targetEntityId &&
                                            !ura.IsDeleted, cancellationToken);

            if (existingAssignment != null)
            {
                // If it exists and is in a pending/rejected state, maybe update it? Or just return success/failure.
                // For now, if it exists and is active, consider it a success or a "no-op".
                if(existingAssignment.AssignmentStatus == request.InitialAssignmentStatus)
                     return true; // Already assigned with the same status

                // If exists but status is different, update status (e.g. re-activating a previously ended assignment)
                existingAssignment.AssignmentStatus = request.InitialAssignmentStatus;
                existingAssignment.VerificationNotes = request.VerificationNotes ?? existingAssignment.VerificationNotes;
                existingAssignment.UpdatedAt = DateTime.UtcNow; // Assuming UserRoleAssignment has UpdatedAt
            }
            else
            {
                var newAssignment = new UserRoleAssignment
                {
                    UserId = userToAssign.Id,
                    RoleId = roleToAssign.Id,
                    TargetEntityId = targetEntityId,
                    AssignmentStatus = request.InitialAssignmentStatus,
                    VerificationNotes = request.VerificationNotes,
                    AssignedAt = DateTime.UtcNow,
                    AssignedByUserId = request.AssignedByUserId
                };
                await _context.UserRoleAssignments.AddAsync(newAssignment, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

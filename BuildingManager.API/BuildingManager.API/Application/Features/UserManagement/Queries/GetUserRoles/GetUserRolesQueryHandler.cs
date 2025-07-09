using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.UserManagement.Queries.GetUserRoles
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<UserRoleResponseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserRoleResponseDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PublicId == request.UserPublicId && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return new List<UserRoleResponseDto>(); // Or throw NotFoundException
            }

            var query = _context.UserRoleAssignments
                .AsNoTracking()
                .Include(ura => ura.Role)
                .Where(ura => ura.UserId == user.Id && !ura.IsDeleted && ura.AssignmentStatus == AssignmentStatus.Active); // Only active assignments

            if (request.TargetEntityPublicId.HasValue && request.TargetEntityType.HasValue)
            {
                int? targetEntityId = null;
                if (request.TargetEntityType == HierarchyLevel.Complex)
                {
                    targetEntityId = await _context.Complexes
                                        .Where(c => c.PublicId == request.TargetEntityPublicId.Value && !c.IsDeleted)
                                        .Select(c => (int?)c.Id)
                                        .FirstOrDefaultAsync(cancellationToken);
                }
                else if (request.TargetEntityType == HierarchyLevel.Block)
                {
                     targetEntityId = await _context.Blocks
                                        .Where(b => b.PublicId == request.TargetEntityPublicId.Value && !b.IsDeleted)
                                        .Select(b => (int?)b.Id)
                                        .FirstOrDefaultAsync(cancellationToken);
                }

                if(targetEntityId.HasValue)
                {
                    query = query.Where(ura => ura.TargetEntityId == targetEntityId.Value);
                }
                else
                {
                     return new List<UserRoleResponseDto>(); // Target entity not found
                }
            }
            else if (request.TargetEntityPublicId.HasValue && !request.TargetEntityType.HasValue)
            {
                // If TargetEntityPublicId is given, TargetEntityType should also be given to resolve the ID.
                // Or, search in both Complexes and Blocks, which is less efficient.
                // For now, assume if PublicId is given, type is also given.
            }
            else
            {
                // No specific target entity, return all active roles (system roles or roles not tied to a specific entity if any)
                // This might need refinement based on how "global" roles vs scoped roles are handled.
                // For now, if TargetEntity is not specified, we might only return System level roles.
                query = query.Where(ura => ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System && ura.TargetEntityId == null);
            }


            var assignments = await query.ToListAsync(cancellationToken);

            var response = new List<UserRoleResponseDto>();
            foreach (var assignment in assignments)
            {
                string? targetEntityName = null;
                if (assignment.TargetEntityId.HasValue)
                {
                    if (assignment.Role.AppliesToHierarchyLevel == HierarchyLevel.Complex)
                    {
                        targetEntityName = await _context.Complexes
                            .Where(c => c.Id == assignment.TargetEntityId.Value)
                            .Select(c => c.Name)
                            .FirstOrDefaultAsync(cancellationToken);
                    }
                    else if (assignment.Role.AppliesToHierarchyLevel == HierarchyLevel.Block)
                    {
                        targetEntityName = await _context.Blocks
                            .Where(b => b.Id == assignment.TargetEntityId.Value)
                            .Select(b => b.NameOrNumber)
                            .FirstOrDefaultAsync(cancellationToken);
                    }
                }

                response.Add(new UserRoleResponseDto
                {
                    RoleName = assignment.Role.Name,
                    RoleNormalizedName = assignment.Role.NormalizedName,
                    RoleDescription = assignment.Role.Description,
                    RoleScope = assignment.Role.AppliesToHierarchyLevel,
                    TargetEntityPublicId = assignment.TargetEntityId.HasValue ?
                        (assignment.Role.AppliesToHierarchyLevel == HierarchyLevel.Complex ?
                            _context.Complexes.Where(c=>c.Id == assignment.TargetEntityId).Select(c=>c.PublicId).FirstOrDefault() :
                         assignment.Role.AppliesToHierarchyLevel == HierarchyLevel.Block ?
                            _context.Blocks.Where(b=>b.Id == assignment.TargetEntityId).Select(b=>b.PublicId).FirstOrDefault() :
                            (Guid?)null)
                        : null,
                    TargetEntityName = targetEntityName,
                    AssignmentStatus = assignment.AssignmentStatus,
                    AssignedAt = assignment.AssignedAt
                });
            }
            return response;
        }
    }
}

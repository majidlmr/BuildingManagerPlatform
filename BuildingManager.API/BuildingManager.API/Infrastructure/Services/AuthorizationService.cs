using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Infrastructure.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IApplicationDbContext _context;

        public AuthorizationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(
            int userId,
            string permissionName,
            HierarchyLevel? entityLevel = null,
            int? targetEntityId = null,
            CancellationToken cancellationToken = default)
        {
            var userRoleAssignmentsQuery = _context.UserRoleAssignments
                .AsNoTracking()
                .Where(ura => ura.UserId == userId &&
                              !ura.IsDeleted &&
                              ura.AssignmentStatus == AssignmentStatus.Active &&
                              !ura.Role.IsDeleted); // Ensure the role itself is not deleted

            if (entityLevel.HasValue && targetEntityId.HasValue)
            {
                // Scoped permission check
                switch (entityLevel.Value)
                {
                    case HierarchyLevel.System:
                        // For system level, targetEntityId should typically be null or not considered.
                        // If a system role is incorrectly assigned with a targetEntityId, this might filter it out.
                        userRoleAssignmentsQuery = userRoleAssignmentsQuery.Where(ura =>
                            ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System &&
                            ura.TargetEntityId == null); // System roles are not tied to an entity
                        break;
                    case HierarchyLevel.Complex:
                        userRoleAssignmentsQuery = userRoleAssignmentsQuery.Where(ura =>
                            (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.Complex && ura.TargetEntityId == targetEntityId.Value) ||
                            (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System && ura.TargetEntityId == null) // System roles apply everywhere
                        );
                        break;
                    case HierarchyLevel.Block:
                        // User has permission if:
                        // 1. Role is System level (applies everywhere)
                        // 2. Role is Complex level AND targetEntityId (Block's Id) belongs to a Complex the user has this role for.
                        // 3. Role is Block level AND targetEntityId matches.

                        // Get ParentComplexId for the given blockId if needed for Complex level role check
                        int? parentComplexIdForBlock = null;
                        if (targetEntityId.HasValue)
                        {
                             parentComplexIdForBlock = await _context.Blocks
                                .Where(b => b.Id == targetEntityId.Value && !b.IsDeleted)
                                .Select(b => b.ParentComplexId)
                                .FirstOrDefaultAsync(cancellationToken);
                        }

                        userRoleAssignmentsQuery = userRoleAssignmentsQuery.Where(ura =>
                            (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.Block && ura.TargetEntityId == targetEntityId.Value) ||
                            (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.Complex && parentComplexIdForBlock.HasValue && ura.TargetEntityId == parentComplexIdForBlock.Value) ||
                            (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System && ura.TargetEntityId == null)
                        );
                        break;
                }
            }
            else // System-level permission check (no specific entity)
            {
                userRoleAssignmentsQuery = userRoleAssignmentsQuery.Where(ura =>
                    ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System &&
                    ura.TargetEntityId == null);
            }

            return await userRoleAssignmentsQuery
                .Include(ura => ura.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .AnyAsync(ura => ura.Role.RolePermissions.Any(rp =>
                    !rp.IsDeleted &&
                    rp.Permission.Name == permissionName &&
                    !rp.Permission.IsDeleted), cancellationToken);
        }

        public async Task<bool> IsAssociatedWithComplexAsync(int userId, int complexId, CancellationToken cancellationToken = default)
        {
            // User is associated if they have an active role directly assigned to this complex
            // or a system-level role.
            return await _context.UserRoleAssignments
                .AsNoTracking()
                .AnyAsync(ura => ura.UserId == userId &&
                                 !ura.IsDeleted &&
                                 ura.AssignmentStatus == AssignmentStatus.Active &&
                                 !ura.Role.IsDeleted &&
                                 ((ura.Role.AppliesToHierarchyLevel == HierarchyLevel.Complex && ura.TargetEntityId == complexId) ||
                                  (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System && ura.TargetEntityId == null)),
                                 cancellationToken);
        }

        public async Task<bool> IsAssociatedWithBlockAsync(int userId, int blockId, CancellationToken cancellationToken = default)
        {
            // User is associated if:
            // 1. Active role directly for this block.
            // 2. Active role for the parent complex of this block.
            // 3. Active system-level role.
            // 4. Active resident of a unit within this block.

            bool hasDirectOrParentRole = await _context.UserRoleAssignments
                .AsNoTracking()
                .Include(ura => ura.Role)
                .Where(ura => ura.UserId == userId && !ura.IsDeleted && ura.AssignmentStatus == AssignmentStatus.Active && !ura.Role.IsDeleted)
                .AnyAsync(ura =>
                    (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.Block && ura.TargetEntityId == blockId) ||
                    (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.Complex && _context.Blocks.Any(b => b.Id == blockId && b.ParentComplexId == ura.TargetEntityId)) ||
                    (ura.Role.AppliesToHierarchyLevel == HierarchyLevel.System && ura.TargetEntityId == null),
                    cancellationToken);

            if (hasDirectOrParentRole) return true;

            return await IsActiveResidentOfUnitInBlockAsync(userId, blockId, cancellationToken);
        }

        private async Task<bool> IsActiveResidentOfUnitInBlockAsync(int userId, int blockId, CancellationToken cancellationToken)
        {
             return await _context.UnitAssignments
                .AsNoTracking()
                .AnyAsync(ua => ua.UserId == userId &&
                                !ua.IsDeleted &&
                                ua.AssignmentStatus == UnitAssignmentStatus.Active &&
                                _context.Units.Any(u => u.Id == ua.UnitId && u.BlockId == blockId && !u.IsDeleted),
                                cancellationToken);
        }


        public async Task<bool> IsActiveResidentOfUnitAsync(int userId, int unitId, CancellationToken cancellationToken = default)
        {
            return await _context.UnitAssignments
                .AsNoTracking()
                .AnyAsync(ua => ua.UserId == userId &&
                                ua.UnitId == unitId &&
                                !ua.IsDeleted &&
                                ua.AssignmentStatus == UnitAssignmentStatus.Active,
                                cancellationToken);
        }

        public async Task<bool> CanAccessTicketAsync(int userId, Guid ticketPublicId, CancellationToken cancellationToken = default)
        {
            var ticketInfo = await _context.Tickets
                .AsNoTracking()
                .Where(t => t.PublicId == ticketPublicId && !t.IsDeleted)
                .Select(t => new { t.Id, t.ReportedByUserId, t.AssignedToUserId, t.BlockId, BlockPublicId = t.Block.PublicId, ComplexId = t.Block.ParentComplexId })
                .FirstOrDefaultAsync(cancellationToken);

            if (ticketInfo == null) return false;
            if (ticketInfo.ReportedByUserId == userId) return true;
            if (ticketInfo.AssignedToUserId == userId) return true;

            // Check permission at Block level
            if (await HasPermissionAsync(userId, "Permissions.Ticket.ViewAllInScope", HierarchyLevel.Block, ticketInfo.BlockId, cancellationToken))
            {
                return true;
            }
            // If ticket's block belongs to a complex, check permission at Complex level
            if (ticketInfo.ComplexId.HasValue &&
                await HasPermissionAsync(userId, "Permissions.Ticket.ViewAllInScope", HierarchyLevel.Complex, ticketInfo.ComplexId.Value, cancellationToken))
            {
                return true;
            }
            // Check for system-level permission
            return await HasPermissionAsync(userId, "Permissions.Ticket.ViewAllInScope", HierarchyLevel.System, null, cancellationToken);
        }

        public async Task<bool> CanAccessInvoiceAsync(int userId, Guid invoicePublicId, CancellationToken cancellationToken = default)
        {
            var invoiceInfo = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Unit) // To get BlockId
                    .ThenInclude(u=> u.Block) // To get ParentComplexId
                .Where(i => i.PublicId == invoicePublicId && !i.IsDeleted)
                .Select(i => new {
                    i.Id,
                    i.UserId, // User to whom invoice is issued
                    UnitId = (int?)i.UnitId,
                    BlockId = (int?)i.Unit.BlockId,
                    ComplexId = (int?)i.Unit.Block.ParentComplexId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (invoiceInfo == null) return false;
            if (invoiceInfo.UserId == userId) return true; // User's own invoice

            // Check permission at Unit level (if a specific permission like "Permissions.Billing.ViewUnitInvoices" exists)
            // For now, let's assume "ViewAllInvoices" is checked at Block/Complex level.

            if (invoiceInfo.BlockId.HasValue &&
                await HasPermissionAsync(userId, "Permissions.Billing.ViewAllInvoices", HierarchyLevel.Block, invoiceInfo.BlockId.Value, cancellationToken))
            {
                return true;
            }
            if (invoiceInfo.ComplexId.HasValue &&
                await HasPermissionAsync(userId, "Permissions.Billing.ViewAllInvoices", HierarchyLevel.Complex, invoiceInfo.ComplexId.Value, cancellationToken))
            {
                return true;
            }
            return await HasPermissionAsync(userId, "Permissions.Billing.ViewAllInvoices", HierarchyLevel.System, null, cancellationToken);
        }
    }
}
using BuildingManager.API.Domain.Entities; // For HierarchyLevel enum
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Common.Interfaces
{
    /// <summary>
    /// Centralized service for authorization checks based on the RBAC system.
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Checks if a user has a specific permission, optionally scoped to a target entity (Complex or Block).
        /// This is the core method for fine-grained permission checks.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <param name="permissionName">The name of the permission required (e.g., "Permissions.Block.Update").</param>
        /// <param name="entityLevel">The level of the target entity (Complex, Block), or null for System-level permissions.</param>
        /// <param name="targetEntityId">The internal ID of the target Complex or Block, or null if not entity-specific.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the user has the permission, otherwise false.</returns>
        Task<bool> HasPermissionAsync(
            int userId,
            string permissionName,
            HierarchyLevel? entityLevel = null,
            int? targetEntityId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user is actively associated with a specific Complex (e.g., has an active role assigned to it).
        /// </summary>
        Task<bool> IsAssociatedWithComplexAsync(int userId, int complexId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user is actively associated with a specific Block (e.g., has an active role assigned to it, or is an active resident).
        /// </summary>
        Task<bool> IsAssociatedWithBlockAsync(int userId, int blockId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user is an active resident (owner or tenant) of a specific Unit.
        /// </summary>
        Task<bool> IsActiveResidentOfUnitAsync(int userId, int unitId, CancellationToken cancellationToken = default);


        // Specific access checks can still be useful for clarity in business logic
        // These will internally use HasPermissionAsync or other checks.

        /// <summary>
        /// Checks if the user can access a specific ticket.
        /// Access is granted if the user reported the ticket, is assigned to it,
        /// or has broader ticket viewing permissions within the ticket's scope (Block/Complex).
        /// </summary>
        Task<bool> CanAccessTicketAsync(int userId, Guid ticketPublicId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the user can access a specific invoice.
        /// Access is granted if the invoice is for the user, or if the user has broader
        /// invoice viewing permissions within the invoice's scope (Unit/Block/Complex).
        /// </summary>
        Task<bool> CanAccessInvoiceAsync(int userId, Guid invoicePublicId, CancellationToken cancellationToken = default);

        // Add more specific checks as needed, e.g.:
        // Task<bool> CanManageUsersInComplexAsync(int currentUserId, int complexId, CancellationToken cancellationToken = default);
        // Task<bool> CanEditBlockDetailsAsync(int currentUserId, int blockId, CancellationToken cancellationToken = default);
    }
}
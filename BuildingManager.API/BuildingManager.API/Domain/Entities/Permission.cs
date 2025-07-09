using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities
{
    /// <summary>
    /// Represents a specific permission in the system.
    /// e.g., "Ticket.Create", "Billing.ViewAllInvoicesInBlock", "User.AssignRoleToBlock"
    /// </summary>
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)] // Increased length for more descriptive permission names like "Module.SubModule.Action"
        public string Name { get; set; } // Unique name of the permission

        [Required]
        [MaxLength(100)]
        public string Module { get; set; } // The module this permission belongs to (e.g., "Billing", "Ticketing", "UserManagement")

        [MaxLength(500)]
        public string? Description { get; set; }

        // Soft delete fields (optional for permissions, as they are usually seeded and rarely deleted)
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }

        // Navigation Property
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities
{
    public enum HierarchyLevel
    {
        System,  // For roles like SuperAdmin
        Complex, // For roles applicable at the Complex level
        Block    // For roles applicable at the Block level
    }

    /// <summary>
    /// Represents a role within the system, defining a set of permissions.
    /// Roles can be scoped to System, Complex, or Block levels.
    /// </summary>
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Complex Manager", "Block Resident", "System Accountant"

        [Required]
        [MaxLength(100)]
        public string NormalizedName { get; set; } // Uppercase, for unique indexing and searching

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public HierarchyLevel AppliesToHierarchyLevel { get; set; } = HierarchyLevel.Block;

        public bool IsSystemRole { get; set; } = false; // Indicates if the role is a built-in system role and not user-editable

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }


        // Navigation Properties
        public ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
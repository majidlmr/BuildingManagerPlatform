using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between Roles and Permissions.
    /// A role can have multiple permissions, and a permission can be assigned to multiple roles.
    /// </summary>
    public class RolePermission
    {
        [Key]
        public int Id { get; set; } // Added a primary key for easier management

        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Required]
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }

        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedByUserId { get; set; } // Optional: User who granted this permission to the role

        // Soft delete fields (though typically permissions are removed from roles rather than soft-deleting the link)
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}
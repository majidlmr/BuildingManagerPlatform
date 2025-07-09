using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities
{
    public enum AssignmentStatus
    {
        PendingVerification, // For manager roles needing platform admin approval
        Verified,            // Verified by platform admin, might need further local approval or becomes active
        Active,              // Role is fully active for the user in the target entity
        Rejected,            // Manager role request rejected by platform admin
        Suspended,           // Temporarily suspended
        Ended                // Assignment has ended (e.g. manager left)
    }

    /// <summary>
    /// Assigns a Role to a User, potentially scoped to a specific Complex or Block.
    /// This replaces the previous UserRole entity.
    /// </summary>
    public class UserRoleAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        /// <summary>
        /// شناسه مجتمع یا بلوکی که این نقش برای کاربر در آن زمینه فعال است.
        /// اگر نقش سیستمی (مثل SuperAdmin) یا عمومی برای کاربر باشد، این می‌تواند null باشد.
        /// </summary>
        public int? TargetEntityId { get; set; }
        // Note: No direct navigation property for TargetEntity to avoid complex polymorphism in EF Core.
        // The type of TargetEntityId (Complex or Block) is inferred from Role.AppliesToHierarchyLevel.

        public AssignmentStatus AssignmentStatus { get; set; } = AssignmentStatus.Active; // Default to Active for non-managerial roles

        [MaxLength(1000)]
        public string? VerificationNotes { get; set; } // Notes from admin regarding verification status

        [Required]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedByUserId { get; set; } // User who assigned this role (e.g. an admin)
        // public User AssignedByUser { get; set; } // Optional: Navigation property

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}
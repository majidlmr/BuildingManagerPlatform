using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities
{
    public enum ResidencyType
    {
        Owner,             // Owner residing in the unit
        Tenant,            // Tenant residing in the unit
        OwnerAndResident,  // Owner who also resides in the unit (might be same as Owner if no separate tenant)
        NonResidentOwner   // Owner who does not reside (unit is rented out or vacant)
    }

    public enum UnitAssignmentStatus
    {
        Active,  // Currently active assignment
        Ended,   // Assignment has ended (e.g., tenant moved out, owner sold)
        Future,  // Assignment is for a future date
        Cancelled // Assignment was cancelled before it started
    }

    /// <summary>
    /// Represents the assignment of a User (as a resident, owner, or tenant) to a Unit.
    /// This entity tracks the history of who is associated with a unit over time.
    /// </summary>
    public class UnitAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        [Required]
        public int UserId { get; set; } // The user (resident, owner, or tenant)
        public User User { get; set; }

        [Required]
        public ResidencyType ResidencyType { get; set; }

        [Required]
        public DateTime StartDate { get; set; } // Date the assignment begins

        public DateTime? EndDate { get; set; } // Date the assignment ends (nullable for ongoing)

        [MaxLength(500)]
        public string? ContractFileUrl { get; set; } // URL to lease agreement or ownership document

        public UnitAssignmentStatus AssignmentStatus { get; set; } = UnitAssignmentStatus.Active;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}

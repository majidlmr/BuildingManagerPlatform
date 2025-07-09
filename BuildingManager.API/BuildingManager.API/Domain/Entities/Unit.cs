using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities
{
    public enum UnitType
    {
        Apartment,
        Shop,
        Office,
        Parking,
        Storage
        // Add other types as needed
    }

    public class Unit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        [Required]
        public int BlockId { get; set; } // FK to Block
        public Block Block { get; set; }

        [Required]
        [MaxLength(20)]
        public string UnitNumber { get; set; } // e.g., "A101", "Commercial Unit 3"

        public int? FloorNumber { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Area { get; set; } // Area in square meters, made non-nullable as it's usually important

        public UnitType UnitType { get; set; }

        public int? NumberOfBedrooms { get; set; } // Applicable for residential units

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }

        // Navigation Properties
        // OwnerUserId and OwnershipStatus are removed, handled by UnitAssignment
        public ICollection<UnitAssignment> Assignments { get; set; } = new List<UnitAssignment>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>(); // Invoices related to this unit
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>(); // Tickets reported for this unit
    }
}
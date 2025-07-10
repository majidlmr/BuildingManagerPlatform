using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities
{
    public enum ComplexType
    {
        Residential,
        Commercial,
        MixedUse,
        // Add other types as needed
    }

    public class Complex
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public ComplexType ComplexType { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Longitude { get; set; }

        public string? Amenities { get; set; } // Could be a JSON string or a separate related entity

        [MaxLength(500)]
        public string? RulesFileUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; } // Assuming User Id is int

        // Navigation Properties
        public ICollection<Block> Blocks { get; set; } = new List<Block>();
        public ICollection<Role> Roles { get; set; } = new List<Role>(); // Roles defined at Complex level
        public ICollection<ManagerAssignment> Managers { get; set; } = new List<ManagerAssignment>(); // Managers assigned to this complex
        public SettlementAccount? SettlementAccount { get; set; } // Optional: A complex might have its own settlement account
    }
}

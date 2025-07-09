using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities
{
    public enum BlockType
    {
        Residential,
        Office,
        Commercial,
        MixedUse
        // Add other types as needed
    }

    /// <summary>
    /// Represents a block within a complex or a standalone building.
    /// </summary>
    public class Block
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid PublicId { get; set; } = Guid.NewGuid();

        public int? ParentComplexId { get; set; } // Nullable FK to Complex
        public Complex? ParentComplex { get; set; }

        [Required]
        [MaxLength(200)]
        public string NameOrNumber { get; set; } // Name or number of the block/building

        public BlockType BlockType { get; set; }

        public int? NumberOfFloors { get; set; }
        public int? TotalUnits { get; set; }
        public int? ConstructionYear { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; } // Address if different from Complex or for standalone blocks

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Longitude { get; set; }

        public string? Amenities { get; set; } // Could be a JSON string

        [MaxLength(100)]
        public string ChargeCalculationStrategyName { get; set; } = "Equal";

        [MaxLength(500)]
        public string? RulesFileUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }

        // Navigation Properties
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public ICollection<Role> Roles { get; set; } = new List<Role>(); // Roles defined at Block level
        public ICollection<ManagerAssignment> Managers { get; set; } = new List<ManagerAssignment>(); // Managers assigned to this block
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public SettlementAccount? SettlementAccount { get; set; } // Optional: A block might have its own settlement account
        public ICollection<RuleAcknowledgment> RuleAcknowledgments { get; set; } = new List<RuleAcknowledgment>();
    }
}
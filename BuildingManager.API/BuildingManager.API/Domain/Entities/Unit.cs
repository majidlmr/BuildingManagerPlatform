// File: Domain/Entities/Unit.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities;

public class Unit
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    public int BuildingId { get; set; }
    public Building Building { get; set; } // Navigation Property to Building

    [Required]
    [MaxLength(20)]
    public string UnitNumber { get; set; }

    [MaxLength(50)]
    public string? UnitType { get; set; }

    public int? FloorNumber { get; set; }
    public int? Bedrooms { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Area { get; set; }

    [Required]
    [MaxLength(50)]
    public string OwnershipStatus { get; set; }

    [Required]
    public int OwnerUserId { get; set; } // Soft-link to the owner User

    // --- Navigation Property اصلاح شده و ضروری ---
    // این خط به EF Core می‌گوید که هر واحد می‌تواند چندین رکورد تخصیص ساکن داشته باشد.
    public ICollection<ResidentAssignment> ResidentAssignments { get; set; } = new List<ResidentAssignment>();
}
using System.ComponentModel.DataAnnotations;
namespace BuildingManager.API.Domain.Entities;

public class Asset
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BlockId { get; set; } // Changed from BuildingId
    public Block Block { get; set; } // Changed from Building
    [Required]
    [MaxLength(50)]
    public string AssetType { get; set; } // "Parking", "Storage"
    [Required]
    [MaxLength(100)]
    public string Identifier { get; set; } // "P-101"
    public string? LocationDescription { get; set; }
    public ICollection<AssetAssignment> Assignments { get; set; } = new List<AssetAssignment>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
using System.ComponentModel.DataAnnotations;
namespace BuildingManager.API.Domain.Entities;

public class Asset
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BuildingId { get; set; }
    public Building Building { get; set; }
    [Required]
    [MaxLength(50)]
    public string AssetType { get; set; } // "Parking", "Storage"
    [Required]
    [MaxLength(100)]
    public string Identifier { get; set; } // "P-101"
    public string? LocationDescription { get; set; }
    public ICollection<AssetAssignment> Assignments { get; set; } = new List<AssetAssignment>();
}
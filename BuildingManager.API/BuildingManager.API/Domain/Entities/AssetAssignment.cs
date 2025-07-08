using System.ComponentModel.DataAnnotations;
namespace BuildingManager.API.Domain.Entities;

public class AssetAssignment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int AssetId { get; set; }
    public Asset Asset { get; set; }
    public int? UnitId { get; set; } // Nullable, as it might be assigned to a person not a unit
    public Unit? Unit { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
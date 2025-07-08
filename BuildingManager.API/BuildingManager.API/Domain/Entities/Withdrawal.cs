using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BuildingManager.API.Domain.Entities;

public class Withdrawal
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BuildingId { get; set; }
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } // "Requested", "Processing", "Completed"
    [Required]
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    [Required]
    [MaxLength(34)]
    public string DestinationIban { get; set; }
    [MaxLength(255)]
    public string? GatewaySettlementId { get; set; }
    public string? IncludedTransactionIds { get; set; } // JSON list of transaction IDs
    public string? Notes { get; set; }
}
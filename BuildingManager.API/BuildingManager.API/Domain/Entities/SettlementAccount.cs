using System.ComponentModel.DataAnnotations;
namespace BuildingManager.API.Domain.Entities;

public class SettlementAccount
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BuildingId { get; set; }
    public Building Building { get; set; }
    [Required]
    [MaxLength(200)]
    public string AccountHolderName { get; set; }
    [Required]
    [MaxLength(100)]
    public string BankName { get; set; }
    [Required]
    [MaxLength(34)]
    public string Iban { get; set; }
    [Required]
    public bool IsDefault { get; set; } = true;
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
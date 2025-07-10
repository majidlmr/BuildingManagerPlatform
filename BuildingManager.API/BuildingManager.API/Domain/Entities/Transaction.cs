using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BuildingManager.API.Domain.Entities;

public class Transaction
{
    [Key]
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    [Required]
    public int PayerUserId { get; set; }
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    [Required]
    [MaxLength(100)]
    public string PaymentGateway { get; set; }
    [Required]
    [MaxLength(255)]
    public string GatewayRefId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } // "Succeeded", "Failed"
    [Required]
    public DateTime PaidAt { get; set; }

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
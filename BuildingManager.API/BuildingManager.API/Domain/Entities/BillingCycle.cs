using System.ComponentModel.DataAnnotations;
namespace BuildingManager.API.Domain.Entities;

public class BillingCycle
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BuildingId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } // e.g., "شارژ تیر ماه ۱۴۰۴"
    [Required]
    public DateTime DueDate { get; set; } // تاریخ سررسید
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
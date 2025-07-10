using BuildingManager.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities;

public class InvoiceItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    [Required]
    public InvoiceItemType Type { get; set; }

    [Required]
    [MaxLength(250)]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; } // Was Amount, renamed based on spec

    // Foreign Key for ExpenseType (Category)
    public int? ExpenseCategoryId { get; set; }
    public ExpenseType? ExpenseCategory { get; set; }

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
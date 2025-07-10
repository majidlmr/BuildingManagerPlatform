using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    public int UnitId { get; set; }
    public Unit Unit { get; set; } // Navigation property اضافه شد

    [Required]
    public int BuildingId { get; set; }

    [Required]
    public int UserId { get; set; } // شناسه مالک یا مستاجری که صورتحساب برای اوست
    public User User { get; set; }   // Navigation property اضافه شد

    [Required]
    public int BillingCycleId { get; set; }
    public BillingCycle BillingCycle { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; } // مبلغ کل، که از جمع آیتم‌ها محاسبه می‌شود

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; }

    [Required]
    public string Description { get; set; }

    // یک صورتحساب شامل چندین ردیف (آیتم) است
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
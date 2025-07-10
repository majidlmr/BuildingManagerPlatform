using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingManager.API.Domain.Enums; // Added for Enums

namespace BuildingManager.API.Domain.Entities;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    public int? ComplexId { get; set; } // Added ComplexId
    public Complex? Complex { get; set; } // Added Navigation for Complex

    [Required]
    public int BlockId { get; set; } // Renamed from BuildingId
    public Block Block { get; set; } // Navigation property should point to Block

    [Required]
    public int UnitId { get; set; }
    public Unit Unit { get; set; }

    [Required]
    public int UserId { get; set; } // شناسه مالک یا مستاجری که صورتحساب برای اوست
    public User User { get; set; }

    [Required]
    public int BillingCycleId { get; set; }
    public BillingCycle BillingCycle { get; set; }

    [Required]
    public InvoiceType InvoiceType { get; set; } // Added InvoiceType

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; } // مبلغ کل، که از جمع آیتم‌ها محاسبه می‌شود

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; } // Added PaymentDate

    [Required]
    public InvoiceStatus Status { get; set; } // Changed to Enum

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Added CreatedAt

    // یک صورتحساب شامل چندین ردیف (آیتم) است
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
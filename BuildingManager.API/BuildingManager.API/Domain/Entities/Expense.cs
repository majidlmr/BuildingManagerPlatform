﻿using BuildingManager.API.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities;

public class Expense
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BlockId { get; set; } // Changed from BuildingId
    public Block Block { get; set; } // Changed from Building

    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ExpenseDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; }

    [Required]
    public AllocationMethod AllocationMethod { get; set; } // فیلد جدید: روش تقسیم هزینه

    public string? ReceiptUrl { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int RecordedByUserId { get; set; }

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
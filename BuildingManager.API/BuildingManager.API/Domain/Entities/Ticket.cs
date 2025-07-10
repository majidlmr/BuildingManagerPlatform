// File: Domain/Entities/Ticket.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace BuildingManager.API.Domain.Entities;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    public int BlockId { get; set; } // Changed from BuildingId
    public Block Block { get; set; } // Changed from Building

    public int? UnitId { get; set; }
    public Unit? Unit { get; set; }

    [Required]
    public int ReportedByUserId { get; set; }
    public User ReportedBy { get; set; }

    // 🚀 فیلد جدید: برای پیاده‌سازی قابلیت ارسال ناشناس
    // اگر این مقدار true باشد، در هنگام نمایش تیکت، هویت ارسال‌کننده مخفی خواهد ماند.
    [Required]
    public bool IsAnonymous { get; set; } = false;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; }

    [Required]
    [MaxLength(50)]
    public string Priority { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public string? AttachmentUrl { get; set; }

    public ICollection<TicketUpdate> Updates { get; set; } = new List<TicketUpdate>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
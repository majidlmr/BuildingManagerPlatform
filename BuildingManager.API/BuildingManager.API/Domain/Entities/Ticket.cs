// File: Domain/Entities/Ticket.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using BuildingManager.API.Domain.Enums; // Added for Enums

namespace BuildingManager.API.Domain.Entities;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    public int? ComplexId { get; set; } // Added ComplexId
    public Complex? Complex { get; set; } // Added Navigation for Complex

    [Required]
    public int BlockId { get; set; }
    public Block Block { get; set; }

    public int? UnitId { get; set; }
    public Unit? Unit { get; set; }

    [Required]
    public int ReportedByUserId { get; set; }
    public User ReportedBy { get; set; }

    public int? AssignedToUserId { get; set; } // Added AssignedToUserId
    public User? AssignedToUser { get; set; } // Added Navigation for AssignedToUser

    // 🚀 فیلد جدید: برای پیاده‌سازی قابلیت ارسال ناشناس
    // اگر این مقدار true باشد، در هنگام نمایش تیکت، هویت ارسال‌کننده مخفی خواهد ماند.
    [Required]
    public bool IsAnonymous { get; set; } = false;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }
    public string? ResolutionDetails { get; set; } // Added ResolutionDetails

    [Required]
    public TicketCategory Category { get; set; } // Changed to Enum

    [Required]
    public TicketPriority Priority { get; set; } // Changed to Enum

    [Required]
    public TicketStatus Status { get; set; } // Changed to Enum

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } // This is LastUpdatedAt from spec
    public DateTime? ResolvedAt { get; set; } // This is ClosedAt from spec (can be debated)

    // public string? AttachmentUrl { get; set; } // Removed, will be replaced by ICollection<TicketAttachment>
    public ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>(); // Added for multiple attachments

    public ICollection<TicketUpdate> Updates { get; set; } = new List<TicketUpdate>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
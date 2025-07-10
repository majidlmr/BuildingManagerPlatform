using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک نظرسنجی یا رای‌گیری در ساختمان است.
/// </summary>
public class Poll
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BlockId { get; set; } // Changed from BuildingId
    public Block Block { get; set; } // Changed from Building

    [Required]
    [MaxLength(500)]
    public string Question { get; set; } // سوال اصلی نظرسنجی

    [Required]
    public bool IsActive { get; set; } = true; // آیا نظرسنجی فعال و در حال رای‌گیری است؟

    public DateTime? EndDate { get; set; } // تاریخ بسته شدن خودکار نظرسنجی (اختیاری)

    [Required]
    public bool IsMultipleChoice { get; set; } = false; // آیا کاربر می‌تواند چند گزینه را انتخاب کند؟

    [Required]
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// لیستی از گزینه‌های قابل انتخاب برای این نظرسنجی.
    /// </summary>
    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

public class ResidentAssignment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UnitId { get; set; }
    public Unit Unit { get; set; }

    [Required]
    public int ResidentUserId { get; set; }
    public User Resident { get; set; } // نام Navigation Property برای وضوح بیشتر تغییر کرد

    [Required]
    public DateTime StartDate { get; set; } // تاریخ شروع سکونت

    public DateTime? EndDate { get; set; } // تاریخ پایان سکونت (اگر Null باشد یعنی قرارداد فعال است)

    [Required]
    public bool IsActive { get; set; } // نشان‌دهنده ساکن فعلی

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
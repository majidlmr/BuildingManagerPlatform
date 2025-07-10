using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک اعلان یا خبر در تابلو اعلانات ساختمان است.
/// </summary>
public class Announcement
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BuildingId { get; set; }
    public Building Building { get; set; }

    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public int CreatedByUserId { get; set; } // کاربری که اعلان را ایجاد کرده
    public User CreatedByUser { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; } // تاریخ انقضای اعلان (اختیاری)

    public bool IsPinned { get; set; } = false; // آیا اعلان پین شده است؟

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
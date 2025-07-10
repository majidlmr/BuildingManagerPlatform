using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر تاییدیه مطالعه و پذیرش یک قانون توسط یک کاربر خاص است.
/// </summary>
public class RuleAcknowledgment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RuleId { get; set; }
    public BuildingRule Rule { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    /// <summary>
    /// تاریخ و زمان دقیق تایید قانون توسط کاربر.
    /// </summary>
    [Required]
    public DateTime AcknowledgedAt { get; set; } = DateTime.UtcNow;

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر رای یک کاربر به یک گزینه خاص در یک نظرسنجی است.
/// </summary>
public class Vote
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PollOptionId { get; set; }
    public PollOption PollOption { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک قانون یا مصوبه در ساختمان است.
/// </summary>
public class BuildingRule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BuildingId { get; set; }
    public Building Building { get; set; }

    /// <summary>
    /// عنوان یا موضوع قانون (مثلاً: قانون مربوط به پارکینگ مهمان).
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    /// <summary>
    /// شرح کامل قانون.
    /// </summary>
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// آیا این قانون فعال و لازم‌الاجرا است؟
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// لیستی از تاییدیه‌های ثبت شده برای این قانون.
    /// </summary>
    public ICollection<RuleAcknowledgment> Acknowledgments { get; set; } = new List<RuleAcknowledgment>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
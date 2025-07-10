using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// موجودیت واسط برای ایجاد رابطه چند به چند بین کاربران و ساختمان‌ها
/// جهت تعیین مدیران یک ساختمان
/// </summary>
public class ManagerAssignment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    [Required]
    public int BuildingId { get; set; }
    public Building Building { get; set; }

    /// <summary>
    /// نقشی که مدیر در ساختمان دارد (مثلا: مدیر اصلی، عضو هیئت مدیره)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Role { get; set; } = "Manager"; // یک مقدار پیش‌فرض

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
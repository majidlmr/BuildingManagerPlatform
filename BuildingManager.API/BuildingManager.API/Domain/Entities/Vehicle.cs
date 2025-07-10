using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک وسیله نقلیه متعلق به یک کاربر است.
/// </summary>
public class Vehicle
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// شناسه کاربری که مالک این وسیله نقلیه است.
    /// </summary>
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    /// <summary>
    /// شماره پلاک کامل خودرو.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string PlateNumber { get; set; } // Renamed from LicensePlate

    /// <summary>
    /// سازنده خودرو (مثلاً: ایران خودرو).
    /// </summary>
    [MaxLength(100)]
    public string? Make { get; set; }

    /// <summary>
    /// مدل خودرو (مثلاً: پژو ۲۰۶).
    /// </summary>
    [MaxLength(100)]
    public string? Model { get; set; }

    /// <summary>
    /// رنگ خودرو.
    /// </summary>
    [MaxLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// توضیحات اضافی.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// شناسه واحدی که این خودرو به آن مرتبط است (اختیاری).
    /// </summary>
    public int? UnitId { get; set; }
    public Unit? Unit { get; set; }

    /// <summary>
    /// آیا این خودرو توسط مدیر تایید شده است؟
    /// </summary>
    public bool IsVerifiedByManager { get; set; } = false;

    [Required]
    public bool IsDefault { get; set; } = true; // آیا این خودروی اصلی کاربر است؟ (Kept as it might be useful)

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
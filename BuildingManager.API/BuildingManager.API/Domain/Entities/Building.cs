using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک ساختمان یا یک مجتمع (که شامل چندین بلوک/ساختمان است).
/// </summary>
public class Building
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } // نام ساختمان یا مجتمع

    [Required]
    [MaxLength(50)]
    public string BuildingType { get; set; } // مثلا: "مسکونی"، "تجاری"

    public int? NumberOfFloors { get; set; }
    public int? TotalUnits { get; set; }
    public int? ConstructionYear { get; set; }
    public string? Address { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? Longitude { get; set; }

    public string? Amenities { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // --- Hierarchical Relationship for Complexes/Blocks ---

    /// <summary>
    /// شناسه والد (اختیاری). اگر این ساختمان یک بلوک باشد، به مجتمع اصلی اشاره می‌کند.
    /// </summary>
    public int? ParentBuildingId { get; set; }
    public Building ParentBuilding { get; set; }

    /// <summary>
    /// لیستی از بلوک‌ها یا ساختمان‌های زیرمجموعه این مجتمع.
    /// </summary>
    public ICollection<Building> SubBuildings { get; set; } = new List<Building>();


    // --- Navigation Properties for Relationships ---

    /// <summary>
    /// ✅ (جدید) لیستی از کاربرانی که به عنوان مدیر در این ساختمان فعالیت دارند.
    /// این رابطه جایگزین فیلد تکی 'OwnerUserId' شده است.
    /// </summary>
    public ICollection<ManagerAssignment> Managers { get; set; } = new List<ManagerAssignment>();

    /// <summary>
    /// ✅ (جدید) لیستی از نقش‌های سفارشی که برای این ساختمان تعریف شده‌اند.
    /// </summary>
    public ICollection<Role> Roles { get; set; } = new List<Role>();

    public ICollection<Unit> Units { get; set; } = new List<Unit>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public SettlementAccount SettlementAccount { get; set; }

    /// <summary>
    /// نام استراتژی محاسبه شارژ برای این ساختمان (مثلا: "Equal" یا "ByArea").
    /// </summary>
    [MaxLength(100)]
    public string ChargeCalculationStrategy { get; set; } = "Equal"; // "Equal" به عنوان مقدار پیش‌فرض

}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک کاربر در سیستم است.
/// این موجودیت برای نگهداری اطلاعات هویتی و ارتباطی کاربر استفاده می‌شود.
/// </summary>
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; }

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } // شماره موبایل، شناسه اصلی برای ورود و ارتباط

    public bool PhoneNumberConfirmed { get; set; } = false;

    [MaxLength(255)]
    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; } = false;

    [Required]
    public string PasswordHash { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // --- Navigation Properties for Relationships ---

    /// <summary>
    /// ✅ (جدید) لیست نقش‌هایی که به این کاربر تخصیص داده شده است.
    /// این رابطه چند به چند، اجازه می‌دهد یک کاربر نقش‌های متفاوتی داشته باشد.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// لیستی از ساختمان‌هایی که این کاربر به عنوان مدیر در آن‌ها فعالیت دارد.
    /// </summary>
    public ICollection<ManagerAssignment> ManagedBuildings { get; set; } = new List<ManagerAssignment>();


    // --- Constructors ---

    /// <summary>
    /// سازنده خالی برای استفاده توسط Entity Framework Core.
    /// </summary>
    public User() { }

    /// <summary>
    /// سازنده عمومی برای ایجاد یک کاربر جدید.
    /// توجه: پارامتر 'role' در اینجا دیگر کاربردی ندارد و در آینده حذف خواهد شد،
    /// زیرا نقش‌ها به صورت پویا مدیریت می‌شوند.
    /// </summary>
    public User(string fullName, string phoneNumber, string passwordHash, string role)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        // فیلد Role دیگر مستقیماً در User ذخیره نمی‌شود، اما سازنده برای سازگاری فعلاً باقی می‌ماند
    }
}
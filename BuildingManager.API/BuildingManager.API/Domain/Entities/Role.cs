using BuildingManager.API.Domain.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک نقش قابل تعریف در یک ساختمان است.
/// </summary>
public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BuildingId { get; set; } // هر نقش متعلق به یک ساختمان خاص است
    public Building Building { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } // نام نقش، مثلا "عضو هیئت مدیره"

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}
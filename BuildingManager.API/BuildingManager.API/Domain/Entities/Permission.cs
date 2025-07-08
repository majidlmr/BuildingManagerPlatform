using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک دسترسی (مجوز) خاص در سیستم است.
/// مثلا: "Ticket.Create", "Billing.View"
/// </summary>
public class Permission
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } // نام منحصر به فرد دسترسی

    public string Description { get; set; }
}
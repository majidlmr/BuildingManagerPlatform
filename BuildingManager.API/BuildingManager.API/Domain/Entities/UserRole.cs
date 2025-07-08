namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// جدول واسط برای رابطه چند به چند بین User و Role.
/// </summary>
public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }
}
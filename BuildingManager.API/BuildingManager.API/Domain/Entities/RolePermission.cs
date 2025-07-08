namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// جدول واسط برای رابطه چند به چند بین Role و Permission.
/// </summary>
public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; }

    public int PermissionId { get; set; }
    public Permission Permission { get; set; }
}
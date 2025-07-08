// File: Domain/Constants/UserRoles.cs

namespace BuildingManager.API.Domain.Constants;

/// <summary>
/// این کلاس ثابت، تمام نقش‌های کاربری در سیستم را به صورت متمرکز تعریف می‌کند.
/// استفاده از این کلاس به جای رشته‌های متنی، از خطاهای تایپی جلوگیری کرده و نگهداری کد را آسان‌تر می‌کند.
/// </summary>
public static class UserRoles
{
    public const string BuildingManager = "BuildingManager";
    public const string Resident = "Resident";
    // در آینده می‌توانید نقش‌های دیگری مانند "Admin" یا "Technician" را به اینجا اضافه کنید.
}
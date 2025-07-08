namespace BuildingManager.API.Domain.Enums;

// روش‌های مختلف تقسیم هزینه را تعریف می‌کند
public enum AllocationMethod
{
    Equal,          // تقسیم مساوی بین تمام واحدها
    ByArea,         // تقسیم بر اساس متراژ
    ByOccupants,    // تقسیم بر اساس تعداد نفرات
    Manual          // تخصیص دستی (برای آینده)
}
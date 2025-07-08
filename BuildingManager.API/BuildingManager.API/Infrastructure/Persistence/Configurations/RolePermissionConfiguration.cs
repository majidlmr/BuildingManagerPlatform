using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی جدول واسط RolePermission برای EF Core
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // تعریف کلید اصلی ترکیبی (Composite Primary Key)
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // تعریف رابطه با نقش: اگر نقش حذف شد، دسترسی‌های آن نیز حذف شود
        builder.HasOne(rp => rp.Role)
               .WithMany(r => r.Permissions)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        // تعریف رابطه با دسترسی: اگر دسترسی حذف شد، تخصیص آن به نقش‌ها نیز حذف شود
        builder.HasOne(rp => rp.Permission)
               .WithMany()
               .HasForeignKey(rp => rp.PermissionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
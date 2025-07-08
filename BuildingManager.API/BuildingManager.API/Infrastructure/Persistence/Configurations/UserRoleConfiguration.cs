using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی جدول واسط UserRole برای EF Core
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // تعریف کلید اصلی ترکیبی (Composite Primary Key)
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // تعریف رابطه با کاربر: اگر کاربر حذف شد، تخصیص نقش‌های او نیز حذف شود
        builder.HasOne(ur => ur.User)
               .WithMany(u => u.UserRoles)
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // تعریف رابطه با نقش: اگر نقش حذف شد، تخصیص آن به کاربران نیز حذف شود
        builder.HasOne(ur => ur.Role)
               .WithMany(r => r.UserRoles)
               .HasForeignKey(ur => ur.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
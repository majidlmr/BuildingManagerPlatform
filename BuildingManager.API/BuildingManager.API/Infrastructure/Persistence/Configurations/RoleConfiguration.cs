using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی موجودیت Role برای EF Core
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles"); // نام جدول
        builder.HasKey(r => r.Id);

        // هر نقش باید نامی منحصر به فرد در سطح یک ساختمان داشته باشد
        builder.HasIndex(r => new { r.BuildingId, r.Name }).IsUnique();

        // رابطه با ساختمان: اگر ساختمان حذف شود، نقش‌های تعریف شده برای آن هم حذف می‌شوند
        builder.HasOne(r => r.Building)
               .WithMany(b => b.Roles)
               .HasForeignKey(r => r.BuildingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
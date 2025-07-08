using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی موجودیت Permission برای EF Core
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(p => p.Id);

        // نام هر دسترسی باید در کل سیستم منحصر به فرد باشد
        builder.HasIndex(p => p.Name).IsUnique();
    }
}
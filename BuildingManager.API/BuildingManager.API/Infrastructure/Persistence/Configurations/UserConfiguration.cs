using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", schema: "identity");
        builder.HasKey(u => u.Id);

        // ایندکس یکتا روی شماره موبایل
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.HasIndex(u => u.PublicId).IsUnique();

        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(255); // دیگر اجباری نیست
    }
}
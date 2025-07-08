using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("Units", schema: "building");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.PublicId).IsUnique();
        builder.HasIndex(u => new { u.BuildingId, u.UnitNumber }).IsUnique();

        builder.Property(u => u.UnitNumber).HasMaxLength(20).IsRequired();
        builder.Property(u => u.OwnershipStatus).HasMaxLength(50).IsRequired();

        // رابطه با ساختمان
        builder.HasOne(u => u.Building)
            .WithMany(b => b.Units)
            .HasForeignKey(u => u.BuildingId)
            .OnDelete(DeleteBehavior.Restrict); // <-- تغییر به Restrict
    }
}
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets", schema: "building");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AssetType).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Identifier).HasMaxLength(100).IsRequired();

        // رابطه با ساختمان
        builder.HasOne(a => a.Building)
            .WithMany(b => b.Assets)
            .HasForeignKey(a => a.BuildingId)
            .OnDelete(DeleteBehavior.Restrict); // <-- تغییر به Restrict
    }
}
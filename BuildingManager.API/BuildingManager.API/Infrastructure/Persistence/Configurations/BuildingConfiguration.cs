using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("Buildings", schema: "building");
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.PublicId).IsUnique();

        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
        builder.Property(b => b.BuildingType).HasMaxLength(50).IsRequired();
        builder.Property(b => b.Latitude).HasColumnType("decimal(9, 6)");
        builder.Property(b => b.Longitude).HasColumnType("decimal(9, 6)");
        builder.HasOne(b => b.ParentBuilding)
          .WithMany(b => b.SubBuildings)
          .HasForeignKey(b => b.ParentBuildingId)
          .IsRequired(false) // چون یک مجتمع والد ندارد
          .OnDelete(DeleteBehavior.Restrict); // برای جلوگیری از حذف تصادفی
    }
}
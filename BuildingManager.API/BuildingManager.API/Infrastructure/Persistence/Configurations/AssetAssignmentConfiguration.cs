using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class AssetAssignmentConfiguration : IEntityTypeConfiguration<AssetAssignment>
{
    public void Configure(EntityTypeBuilder<AssetAssignment> builder)
    {
        builder.ToTable("AssetAssignments", schema: "building");
        builder.HasKey(aa => aa.Id);

        // رابطه با دارایی
        builder.HasOne(aa => aa.Asset)
               .WithMany(a => a.Assignments)
               .HasForeignKey(aa => aa.AssetId)
               .OnDelete(DeleteBehavior.Cascade); // اگر خود دارایی حذف شد، تخصیص آن هم حذف شود

        // رابطه با واحد (این رابطه مشکل‌ساز بود)
        builder.HasOne(aa => aa.Unit)
               .WithMany()
               .HasForeignKey(aa => aa.UnitId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull); // <-- تغییر به SetNull
    }
}
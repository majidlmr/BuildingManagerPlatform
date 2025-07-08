using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class BuildingRuleConfiguration : IEntityTypeConfiguration<BuildingRule>
{
    public void Configure(EntityTypeBuilder<BuildingRule> builder)
    {
        builder.ToTable("BuildingRules");
        builder.HasKey(r => r.Id);

        // رابطه با کاربر سازنده: اگر کاربر حذف شد، قوانینی که ساخته هم حذف شوند
        builder.HasOne(r => r.CreatedByUser)
               .WithMany()
               .HasForeignKey(r => r.CreatedByUserId)
               .OnDelete(DeleteBehavior.Cascade);

        // رابطه با ساختمان
        builder.HasOne(r => r.Building)
               .WithMany()
               .HasForeignKey(r => r.BuildingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
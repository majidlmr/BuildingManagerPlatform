using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.ToTable("Polls");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Question)
            .HasMaxLength(500)
            .IsRequired();

        // رابطه با کاربر سازنده: اگر کاربر حذف شد، نظرسنجی‌هایش هم حذف شوند
        builder.HasOne(p => p.CreatedByUser)
               .WithMany()
               .HasForeignKey(p => p.CreatedByUserId)
               .OnDelete(DeleteBehavior.Cascade); // ✅ این مسیر حذف آبشاری اول است

        // رابطه با ساختمان
        builder.HasOne(p => p.Building)
               .WithMany()
               .HasForeignKey(p => p.BuildingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
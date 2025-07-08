using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder.ToTable("PollOptions");
        builder.HasKey(po => po.Id);

        builder.Property(po => po.Text)
            .HasMaxLength(250)
            .IsRequired();

        // رابطه با نظرسنجی: اگر نظرسنجی حذف شد، گزینه‌هایش هم حذف شوند
        builder.HasOne(po => po.Poll)
               .WithMany(p => p.Options)
               .HasForeignKey(po => po.PollId)
               .OnDelete(DeleteBehavior.Cascade); // ✅ این رابطه، مسیر آبشاری را ادامه می‌دهد
    }
}
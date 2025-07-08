using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class TicketUpdateConfiguration : IEntityTypeConfiguration<TicketUpdate>
{
    public void Configure(EntityTypeBuilder<TicketUpdate> builder)
    {
        builder.ToTable("TicketUpdates", schema: "building");
        builder.HasKey(tu => tu.Id);

        builder.HasOne(tu => tu.Ticket)
               .WithMany(t => t.Updates)
               .HasForeignKey(tu => tu.TicketId)
               .OnDelete(DeleteBehavior.Cascade); // اگر تیکت حذف شد، آپدیت‌ها هم حذف شوند

        // حل مشکل مسیر آبشاری چندگانه
        builder.HasOne(tu => tu.UpdateBy)
               .WithMany()
               .HasForeignKey(tu => tu.UpdateByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
// File: Infrastructure/Persistence/Configurations/TicketConfiguration.cs
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

// این کلاس که فراموش شده بود، اکنون اضافه شده است
public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    // متد Configure اکنون به درستی در داخل کلاس قرار دارد
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets", schema: "building");

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.PublicId).IsUnique();

        builder.Property(t => t.Title).HasMaxLength(255).IsRequired();
        builder.Property(t => t.Category).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Priority).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Status).HasMaxLength(50).IsRequired();

        // رابطه با ساختمان: اگر ساختمان حذف شد، تیکت‌هایش هم حذف شوند
        builder.HasOne(t => t.Building)
               .WithMany(b => b.Tickets)
               .HasForeignKey(t => t.BuildingId)
               .OnDelete(DeleteBehavior.Cascade);

        // رابطه با واحد (حل مشکل مسیر آبشاری)
        builder.HasOne(t => t.Unit)
               .WithMany()
               .HasForeignKey(t => t.UnitId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        // رابطه با کاربر گزارش دهنده (حل مشکل مسیر آبشاری)
        builder.HasOne(t => t.ReportedBy)
               .WithMany()
               .HasForeignKey(t => t.ReportedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
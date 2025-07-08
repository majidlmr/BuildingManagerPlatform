using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class SettlementAccountConfiguration : IEntityTypeConfiguration<SettlementAccount>
{
    public void Configure(EntityTypeBuilder<SettlementAccount> builder)
    {
        builder.ToTable("SettlementAccounts", schema: "building");
        builder.HasKey(sa => sa.Id);

        // هر ساختمان فقط یک حساب تسویه پیش‌فرض دارد
        builder.HasIndex(sa => sa.BuildingId).IsUnique();

        builder.Property(sa => sa.Iban).HasMaxLength(34).IsRequired();

        // تعریف رابطه یک به یک با ساختمان
        builder.HasOne(sa => sa.Building)
            .WithOne(b => b.SettlementAccount)
            .HasForeignKey<SettlementAccount>(sa => sa.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
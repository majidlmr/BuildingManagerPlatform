using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class WithdrawalConfiguration : IEntityTypeConfiguration<Withdrawal>
{
    public void Configure(EntityTypeBuilder<Withdrawal> builder)
    {
        builder.ToTable("Withdrawals", schema: "billing");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Amount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(w => w.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.DestinationIban)
            .HasMaxLength(34)
            .IsRequired();

        builder.Property(w => w.GatewaySettlementId)
            .HasMaxLength(255);
    }
}
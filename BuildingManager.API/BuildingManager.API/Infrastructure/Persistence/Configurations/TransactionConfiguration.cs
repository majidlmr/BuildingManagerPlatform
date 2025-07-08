// File: Infrastructure/Persistence/Configurations/TransactionConfiguration.cs
using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions", schema: "billing");
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.GatewayRefId).IsUnique();
        builder.Property(t => t.Amount).HasColumnType("decimal(18, 2)").IsRequired();

        // این کد اکنون به درستی کار خواهد کرد چون پراپرتی Transactions در کلاس Invoice وجود دارد
        builder.HasOne(t => t.Invoice)
            .WithMany(i => i.Transactions)
            .HasForeignKey(t => t.InvoiceId)
            .IsRequired(false) // چون ممکن است پرداخت مستقیم و بدون فاکتور باشد
            .OnDelete(DeleteBehavior.SetNull); // اگر فاکتور حذف شد، تراکنش باقی بماند
    }
}
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Enums; // Added for Enums
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", schema: "billing"); // Assuming "billing" schema exists or is desired
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.PublicId).IsUnique();

        builder.Property(i => i.Amount).HasColumnType("decimal(18, 2)").IsRequired();

        builder.Property(i => i.InvoiceType)
            .HasConversion<string>()
            .HasMaxLength(50) // Ensure length is sufficient for enum string values
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(50) // Ensure length is sufficient
            .IsRequired();

        // Relation to Block (previously Building)
        builder.HasOne(i => i.Block)
            .WithMany(b => b.Invoices) // Assuming Block has ICollection<Invoice> Invoices
            .HasForeignKey(i => i.BlockId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on business rules

        // Relation to Unit
        builder.HasOne(i => i.Unit)
            .WithMany(u => u.Invoices) // Assuming Unit has ICollection<Invoice> Invoices
            .HasForeignKey(i => i.UnitId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Relation to User (who the invoice is for)
        builder.HasOne(i => i.User)
            .WithMany() // Assuming User doesn't have a specific collection for "InvoicesToPay"
            .HasForeignKey(i => i.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Relation to Complex (Optional)
        builder.HasOne(i => i.Complex)
            .WithMany() // Assuming Complex doesn't have a direct ICollection<Invoice>
            .HasForeignKey(i => i.ComplexId)
            .IsRequired(false) // ComplexId is nullable
            .OnDelete(DeleteBehavior.ClientSetNull); // Or Restrict

        // Relation to BillingCycle
        builder.HasOne(i => i.BillingCycle)
            .WithMany() // Assuming BillingCycle doesn't have ICollection<Invoice>
            .HasForeignKey(i => i.BillingCycleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Note: Relationships for Items (InvoiceItem) and Transactions are typically configured
        // from the "many" side (InvoiceItemConfiguration, TransactionConfiguration) using HasOne for Invoice.
    }
}
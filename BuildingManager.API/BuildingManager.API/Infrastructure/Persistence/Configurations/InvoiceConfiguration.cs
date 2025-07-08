using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", schema: "billing");
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.PublicId).IsUnique();
        builder.Property(i => i.Amount).HasColumnType("decimal(18, 2)").IsRequired();
    }
}
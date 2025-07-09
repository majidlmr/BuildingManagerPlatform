using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Units", schema: "building");

            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.PublicId).IsUnique();

            // Unique constraint for UnitNumber within a Block
            builder.HasIndex(u => new { u.BlockId, u.UnitNumber }).IsUnique();

            builder.Property(u => u.UnitNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.UnitType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(u => u.Area)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(u => !u.IsDeleted);

            // Relationship to Block (Many-to-One)
            builder.HasOne(u => u.Block)
                .WithMany(b => b.Units)
                .HasForeignKey(u => u.BlockId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict. Deleting a block with units should be handled carefully.

            // Relationships for Assignments, Invoices, Tickets
            builder.HasMany(u => u.Assignments)
                .WithOne(a => a.Unit) // Assuming UnitAssignment has a Unit navigation property
                .HasForeignKey(a => a.UnitId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Invoices)
                .WithOne(i => i.Unit) // Assuming Invoice has a Unit navigation property
                .HasForeignKey(i => i.UnitId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Don't delete invoices if unit is deleted, handle this logic elsewhere

            builder.HasMany(u => u.Tickets)
                .WithOne(t => t.Unit) // Assuming Ticket has a Unit navigation property
                .HasForeignKey(t => t.UnitId)
                .IsRequired(false) // A ticket might not always be for a specific unit (e.g. common area)
                .OnDelete(DeleteBehavior.SetNull); // If unit is deleted, set UnitId in Ticket to null
        }
    }
}
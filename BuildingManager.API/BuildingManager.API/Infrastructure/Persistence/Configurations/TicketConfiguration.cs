// File: Infrastructure/Persistence/Configurations/TicketConfiguration.cs
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Enums; // Added for Enums
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets", schema: "building"); // Assuming "building" schema

        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.PublicId).IsUnique();

        builder.Property(t => t.Title).HasMaxLength(255).IsRequired();

        builder.Property(t => t.Category)
            .HasConversion<string>()
            .HasMaxLength(100) // Keep existing length or adjust for enum string values
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(50) // Keep existing length or adjust
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50) // Keep existing length or adjust
            .IsRequired();

        // Relation to Block (Corrected from Building)
        builder.HasOne(t => t.Block)
               .WithMany(b => b.Tickets) // Assuming Block has ICollection<Ticket> Tickets
               .HasForeignKey(t => t.BlockId) // Correct FK name
               .IsRequired() // A ticket must belong to a block
               .OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on rules

        // Relation to Unit (Optional)
        builder.HasOne(t => t.Unit)
               .WithMany(u => u.Tickets) // Assuming Unit has ICollection<Ticket> Tickets
               .HasForeignKey(t => t.UnitId)
               .IsRequired(false) // UnitId is nullable
               .OnDelete(DeleteBehavior.ClientSetNull); // If Unit is deleted, set UnitId to null

        // Relation to User (ReportedBy)
        builder.HasOne(t => t.ReportedBy)
               .WithMany() // Assuming User doesn't have a specific collection for "ReportedTickets"
               .HasForeignKey(t => t.ReportedByUserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Relation to User (AssignedToUser) (Optional)
        builder.HasOne(t => t.AssignedToUser)
            .WithMany() // Assuming User doesn't have a specific collection for "AssignedTickets"
            .HasForeignKey(t => t.AssignedToUserId)
            .IsRequired(false) // AssignedToUserId is nullable
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Relation to Complex (Optional)
        builder.HasOne(t => t.Complex)
            .WithMany() // Assuming Complex doesn't have a direct ICollection<Ticket>
            .HasForeignKey(t => t.ComplexId)
            .IsRequired(false) // ComplexId is nullable
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Relation to TicketAttachment (One-to-Many)
        builder.HasMany(t => t.Attachments)
            .WithOne(ta => ta.Ticket)
            .HasForeignKey(ta => ta.TicketId)
            .OnDelete(DeleteBehavior.Cascade); // If a ticket is deleted, its attachments are also deleted.

        // Note: Relationship for Updates (TicketUpdate) would be configured similarly
        // from TicketUpdateConfiguration using HasOne for Ticket.
    }
}
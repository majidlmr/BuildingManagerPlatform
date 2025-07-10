using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachment>
    {
        public void Configure(EntityTypeBuilder<TicketAttachment> builder)
        {
            builder.ToTable("TicketAttachments", schema: "building"); // Or your preferred schema

            builder.HasKey(ta => ta.Id);

            builder.Property(ta => ta.FileUrl)
                .HasMaxLength(1024) // Consistent with entity
                .IsRequired();

            builder.Property(ta => ta.FileName)
                .HasMaxLength(255); // Consistent with entity (if we had added it)

            builder.Property(ta => ta.ContentType)
                .HasMaxLength(100); // Consistent with entity (if we had added it)

            // Relationship with Ticket (Many-to-One)
            // This is already configured in TicketConfiguration via HasMany.
            // However, explicitly defining the other end can be done for clarity or specific needs.
            // builder.HasOne(ta => ta.Ticket)
            //        .WithMany(t => t.Attachments)
            //        .HasForeignKey(ta => ta.TicketId)
            //        .OnDelete(DeleteBehavior.Cascade); // Ensure this matches TicketConfiguration

            // Relationship with User (UploadedByUser) (Many-to-One)
            builder.HasOne(ta => ta.UploadedByUser)
                   .WithMany() // Assuming User does not have a direct ICollection<TicketAttachment> UploadedAttachments
                   .HasForeignKey(ta => ta.UploadedByUserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a user if they have uploaded attachments
        }
    }
}

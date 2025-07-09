using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class UnitAssignmentConfiguration : IEntityTypeConfiguration<UnitAssignment>
    {
        public void Configure(EntityTypeBuilder<UnitAssignment> builder)
        {
            builder.ToTable("UnitAssignments", schema: "building");

            builder.HasKey(ua => ua.Id);

            // A user should ideally have only one active assignment to a specific unit at a time.
            // This can be enforced with a more complex unique index or application logic.
            // For example, (UnitId, UserId, Active) where Active is true, should be unique.
            // Or (UnitId, Active) where Active is true, allowing only one active resident per unit.
            // This depends on business rules (e.g. can a unit have multiple "active" tenants/owners simultaneously?)
            // For now, we'll rely on application logic to manage overlapping assignments if needed.
            // A simple index for querying:
            builder.HasIndex(ua => new { ua.UnitId, ua.UserId, ua.AssignmentStatus });


            builder.Property(ua => ua.ResidencyType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(ua => ua.StartDate)
                .IsRequired();

            builder.Property(ua => ua.ContractFileUrl)
                .HasMaxLength(500);

            builder.Property(ua => ua.AssignmentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(ua => ua.CreatedAt)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(ua => !ua.IsDeleted);

            // Relationships
            builder.HasOne(ua => ua.Unit)
                .WithMany(u => u.Assignments)
                .HasForeignKey(ua => ua.UnitId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete unit if assignments exist, handle manually or set to Cascade if appropriate

            builder.HasOne(ua => ua.User)
                .WithMany(u => u.UnitAssignments)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete user if assignments exist
        }
    }
}

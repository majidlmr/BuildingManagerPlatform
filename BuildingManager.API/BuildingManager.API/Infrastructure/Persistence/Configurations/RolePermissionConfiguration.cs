using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions", schema: "identity");

            builder.HasKey(rp => rp.Id); // Changed from composite key to single PK

            // A role should have a specific permission only once.
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();

            builder.Property(rp => rp.AssignedAt)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(rp => !rp.IsDeleted);

            // Relationships
            builder.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions) // Corrected navigation property name in Role entity
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // If Role is deleted, this link is removed.

            builder.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions) // Corrected navigation property name in Permission entity
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade); // If Permission is deleted, this link is removed.
        }
    }
}
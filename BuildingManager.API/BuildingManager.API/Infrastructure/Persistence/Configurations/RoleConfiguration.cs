using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", schema: "identity");

            builder.HasKey(r => r.Id);

            // Unique index for NormalizedName.
            // For Complex/Block specific roles, true uniqueness also involves TargetEntityId in UserRoleAssignment,
            // which is enforced at the application logic or a more complex setup if roles were directly tied to entities.
            // Here, we assume NormalizedName should be unique or unique per AppliesToHierarchyLevel.
            // For global uniqueness of role names (e.g. "Super Admin", "Complex Manager")
            builder.HasIndex(r => r.NormalizedName).IsUnique();
            // If role names can be repeated across different hierarchy levels (e.g. "Board Member" for Complex and "Board Member" for Block are distinct roles)
            // then a composite index might be: builder.HasIndex(r => new { r.NormalizedName, r.AppliesToHierarchyLevel }).IsUnique();

            builder.Property(r => r.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.NormalizedName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.AppliesToHierarchyLevel)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.IsSystemRole)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(r => !r.IsDeleted);

            // Relationships
            builder.HasMany(r => r.UserRoleAssignments)
                .WithOne(ura => ura.Role)
                .HasForeignKey(ura => ura.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // If a role is deleted, assignments are removed

            builder.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // If a role is deleted, its permissions links are removed

            // The direct link from Role to a specific Building (now Block/Complex) is removed.
            // Roles are now scoped by AppliesToHierarchyLevel and linked to entities via UserRoleAssignment.TargetEntityId.
        }
    }
}
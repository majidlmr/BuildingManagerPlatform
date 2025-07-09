using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
    {
        public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
        {
            builder.ToTable("UserRoleAssignments", schema: "identity");

            builder.HasKey(ura => ura.Id);

            // A user can have a specific role only once for a specific target entity.
            // If TargetEntityId is null (for System roles), then UserId-RoleId must be unique.
            // This complex uniqueness might be better handled at application logic or via a filtered index if DB supports.
            // For now, a composite index on UserId, RoleId, TargetEntityId.
            builder.HasIndex(ura => new { ura.UserId, ura.RoleId, ura.TargetEntityId }).IsUnique();

            builder.Property(ura => ura.AssignmentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(ura => ura.VerificationNotes)
                .HasMaxLength(1000);

            builder.Property(ura => ura.AssignedAt)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(ura => !ura.IsDeleted);

            // Relationships are configured in UserConfiguration and RoleConfiguration (HasMany)
            // Explicitly define the relationships from this side as well for clarity if needed,
            // but EF Core can usually infer them if one side is configured.

            builder.HasOne(ura => ura.User)
                .WithMany(u => u.UserRoleAssignments)
                .HasForeignKey(ura => ura.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Standard behavior

            builder.HasOne(ura => ura.Role)
                .WithMany(r => r.UserRoleAssignments)
                .HasForeignKey(ura => ura.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Standard behavior

            // Note: TargetEntityId is not a direct FK to Complex or Block table here
            // to avoid polymorphic FKs which EF Core doesn't support well.
            // The logic to interpret TargetEntityId will be in the application layer
            // based on Role.AppliesToHierarchyLevel.
        }
    }
}

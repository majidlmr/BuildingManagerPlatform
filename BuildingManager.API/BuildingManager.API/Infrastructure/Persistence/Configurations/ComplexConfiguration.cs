using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class ComplexConfiguration : IEntityTypeConfiguration<Complex>
    {
        public void Configure(EntityTypeBuilder<Complex> builder)
        {
            builder.ToTable("Complexes", schema: "building"); // Define schema if needed, e.g., "management"

            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.PublicId).IsUnique();

            builder.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.ComplexType)
                .IsRequired()
                .HasConversion<string>() // Store enum as string
                .HasMaxLength(50);

            builder.Property(c => c.Latitude)
                .HasColumnType("decimal(9, 6)");

            builder.Property(c => c.Longitude)
                .HasColumnType("decimal(9, 6)");

            builder.Property(c => c.Amenities)
                .HasColumnType("nvarchar(max)"); // Assuming JSON or long string

            builder.Property(c => c.RulesFileUrl)
                .HasMaxLength(500);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(c => !c.IsDeleted);

            // Relationships
            builder.HasMany(c => c.Blocks)
                .WithOne(b => b.ParentComplex)
                .HasForeignKey(b => b.ParentComplexId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting complex if it has blocks; handle manually

            builder.HasMany(c => c.Roles)
                .WithOne() // Assuming Role can be tied to Complex OR Block, adjust if Role always needs a Building/Block
                .HasForeignKey("ComplexIdForRoles") // Shadow property or explicit FK in Role if Complex-specific
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Managers)
                .WithOne() // Assuming ManagerAssignment can be tied to Complex OR Block
                .HasForeignKey("ComplexIdForManagers") // Shadow property or explicit FK
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship to SettlementAccount (One-to-One, Optional)
            // Assuming SettlementAccount has a ComplexId FK
            // builder.HasOne(c => c.SettlementAccount)
            //     .WithOne(sa => sa.Complex)
            //     .HasForeignKey<SettlementAccount>(sa => sa.ComplexId)
            //     .IsRequired(false);
        }
    }
}

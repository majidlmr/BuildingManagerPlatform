using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class BlockConfiguration : IEntityTypeConfiguration<Block>
    {
        public void Configure(EntityTypeBuilder<Block> builder)
        {
            builder.ToTable("Blocks", schema: "building"); // Renamed table

            builder.HasKey(b => b.Id);
            builder.HasIndex(b => b.PublicId).IsUnique();

            builder.Property(b => b.NameOrNumber)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(b => b.BlockType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(b => b.Address)
                .HasMaxLength(500);

            builder.Property(b => b.Latitude)
                .HasColumnType("decimal(9, 6)");

            builder.Property(b => b.Longitude)
                .HasColumnType("decimal(9, 6)");

            builder.Property(b => b.Amenities)
                .HasColumnType("nvarchar(max)");

            builder.Property(b => b.ChargeCalculationStrategyName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.RulesFileUrl)
                .HasMaxLength(500);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(b => !b.IsDeleted);

            // Relationship to Parent Complex (Optional)
            builder.HasOne(b => b.ParentComplex)
                .WithMany(c => c.Blocks)
                .HasForeignKey(b => b.ParentComplexId)
                .IsRequired(false) // A block can be standalone (not part of a complex)
                .OnDelete(DeleteBehavior.Restrict); // Or SetNull if you want to orphan blocks

            // Relationship to Units (One-to-Many)
            builder.HasMany(b => b.Units)
                .WithOne(u => u.Block)
                .HasForeignKey(u => u.BlockId)
                .IsRequired() // A unit must belong to a block
                .OnDelete(DeleteBehavior.Cascade); // If a block is deleted, its units are also deleted (consider soft delete implications)

            // Relationships for Roles, Managers, Tickets, Assets, SettlementAccount, RuleAcknowledgments
            // These would be similar to ComplexConfiguration, but linking to BlockId
            // Example for Roles:
            builder.HasMany(b => b.Roles)
                .WithOne() // Assuming Role can be tied to Complex OR Block
                .HasForeignKey("BlockIdForRoles") // Shadow property or explicit FK in Role
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Example for Managers:
            builder.HasMany(b => b.Managers)
                .WithOne() // Assuming ManagerAssignment can be tied to Complex OR Block
                .HasForeignKey("BlockIdForManagers") // Shadow property or explicit FK
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Other relationships (Tickets, Assets, SettlementAccount) would follow a similar pattern
            // if they are directly related to Block and not through Unit or another entity.
            // For example, if Tickets are directly reported against a Block:
            builder.HasMany(b => b.Tickets)
                .WithOne(t => t.Block) // Assuming Ticket has a Block navigation property
                .HasForeignKey(t => t.BlockId) // And a BlockId foreign key
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
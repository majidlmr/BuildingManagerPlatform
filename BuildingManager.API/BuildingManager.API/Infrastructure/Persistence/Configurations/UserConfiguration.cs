using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", schema: "identity");

            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.PublicId).IsUnique();
            builder.HasIndex(u => u.NationalId).IsUnique();
            builder.HasIndex(u => u.PhoneNumber).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique().HasFilter("[Email] IS NOT NULL"); // Unique constraint only for non-null emails

            builder.Property(u => u.FirstName)
                .HasMaxLength(75)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(75)
                .IsRequired();

            builder.Property(u => u.NationalId)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.ProfilePictureUrl)
                .HasMaxLength(500);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .IsRequired();

            // Configure soft delete query filter
            builder.HasQueryFilter(u => !u.IsDeleted);

            // Relationships
            builder.HasMany(u => u.UserRoleAssignments)
                .WithOne(ura => ura.User)
                .HasForeignKey(ura => ura.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their role assignments are also deleted

            builder.HasMany(u => u.UnitAssignments)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, their unit assignments are also deleted

            builder.HasMany(u => u.SettlementAccounts)
                .WithOne(sa => sa.User)
                .HasForeignKey(sa => sa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Vehicles)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
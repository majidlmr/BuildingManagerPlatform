using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class ResidentAssignmentConfiguration : IEntityTypeConfiguration<ResidentAssignment>
{
    public void Configure(EntityTypeBuilder<ResidentAssignment> builder)
    {
        builder.ToTable("ResidentAssignments", schema: "building");
        builder.HasKey(ra => ra.Id);

        builder.HasOne(ra => ra.Unit)
            .WithMany()
            .HasForeignKey(ra => ra.UnitId)
            .OnDelete(DeleteBehavior.Cascade); // اگر واحد حذف شد، تخصیص آن هم حذف شود
    }
}
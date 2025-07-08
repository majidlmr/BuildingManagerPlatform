using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class RuleAcknowledgmentConfiguration : IEntityTypeConfiguration<RuleAcknowledgment>
{
    public void Configure(EntityTypeBuilder<RuleAcknowledgment> builder)
    {
        builder.ToTable("RuleAcknowledgments");
        builder.HasKey(ra => ra.Id);

        // رابطه با قانون: اگر قانون حذف شد، تاییدیه هم حذف شود
        builder.HasOne(ra => ra.Rule)
               .WithMany(r => r.Acknowledgments)
               .HasForeignKey(ra => ra.RuleId)
               .OnDelete(DeleteBehavior.Cascade);

        // ✅✅ مهم‌ترین بخش: شکستن چرخه
        // اگر کاربری که قانون را تایید کرده حذف شود، خود تاییدیه حذف نمی‌شود
        builder.HasOne(ra => ra.User)
               .WithMany()
               .HasForeignKey(ra => ra.UserId)
               .OnDelete(DeleteBehavior.Restrict); // 👈 جلوی مسیر دوم را می‌گیرد
    }
}
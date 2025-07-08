using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.ToTable("Votes");
        builder.HasKey(v => v.Id);

        // رابطه با PollOption: اگر گزینه حذف شد، رای هم حذف شود
        builder.HasOne(v => v.PollOption)
               .WithMany(po => po.Votes)
               .HasForeignKey(v => v.PollOptionId)
               .OnDelete(DeleteBehavior.Cascade);

        // ✅✅ مهم‌ترین بخش: شکستن چرخه حذف آبشاری
        // اگر کاربری که رای داده حذف شود، خود رای حذف نمی‌شود (و این مشکلی ندارد)
        builder.HasOne(v => v.User)
               .WithMany()
               .HasForeignKey(v => v.UserId)
               .OnDelete(DeleteBehavior.Restrict); // 👈 جلوی مسیر دوم را می‌گیرد
    }
}
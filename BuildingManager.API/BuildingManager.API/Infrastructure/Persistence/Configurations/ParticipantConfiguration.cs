using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants", schema: "chat");
        builder.HasKey(p => p.Id);

        // جلوگیری از اضافه شدن یک کاربر به یک گفتگو بیش از یک بار
        builder.HasIndex(p => new { p.ConversationId, p.UserId }).IsUnique();

        // تعریف رابطه با گفتگو: هر شرکت‌کننده متعلق به یک گفتگو است
        builder.HasOne(p => p.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade); // اگر گفتگو حذف شد، شرکت‌کنندگان هم حذف شوند
    }
}
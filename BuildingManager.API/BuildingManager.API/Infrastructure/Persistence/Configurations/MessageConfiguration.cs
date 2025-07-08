using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages", schema: "chat");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content).IsRequired();

        // تعریف رابطه با گفتگو: هر پیام متعلق به یک گفتگو است
        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade); // اگر گفتگو حذف شد، پیام‌هایش هم حذف شوند
    }
}
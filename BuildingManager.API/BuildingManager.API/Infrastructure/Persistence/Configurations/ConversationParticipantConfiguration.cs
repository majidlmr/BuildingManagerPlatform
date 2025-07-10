using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant> // Renamed class and type
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder) // Changed type
    {
        builder.ToTable("ConversationParticipants", schema: "chat"); // Changed table name to be more specific
        builder.HasKey(p => p.Id);

        // جلوگیری از اضافه شدن یک کاربر به یک گفتگو بیش از یک بار
        builder.HasIndex(p => new { p.ConversationId, p.UserId }).IsUnique();

        // تعریف رابطه با گفتگو: هر شرکت‌کننده متعلق به یک گفتگو است
        builder.HasOne(p => p.Conversation)
            .WithMany(c => c.ConversationParticipants) // Changed from c.Participants
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade); // اگر گفتگو حذف شد، شرکت‌کنندگان هم حذف شوند

        // رابطه با کاربر (اختیاری، اما خوب است که باشد)
        // builder.HasOne<User>() // Assuming User entity exists
        //     .WithMany() // Assuming User does not have a direct collection of ConversationParticipants
        //     .HasForeignKey(p => p.UserId)
        //     .OnDelete(DeleteBehavior.Restrict); // Or Cascade, depending on requirements

        // رابطه با پیام (برای LastReadMessageId)
        builder.HasOne(p => p.LastReadMessage)
            .WithMany() // Assuming Message does not have a direct back-reference collection for this
            .HasForeignKey(p => p.LastReadMessageId)
            .OnDelete(DeleteBehavior.SetNull); // If message is deleted, don't delete participant, just nullify
    }
}
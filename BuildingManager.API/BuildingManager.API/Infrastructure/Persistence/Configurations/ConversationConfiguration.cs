using BuildingManager.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingManager.API.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations", schema: "chat");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.PublicId).IsUnique();

        builder.HasMany(c => c.Participants)
               .WithOne(p => p.Conversation)
               .HasForeignKey(p => p.ConversationId);

        builder.HasMany(c => c.Messages)
               .WithOne(m => m.Conversation)
               .HasForeignKey(m => m.ConversationId);
    }
}
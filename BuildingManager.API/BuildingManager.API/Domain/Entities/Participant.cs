using System.ComponentModel.DataAnnotations;
namespace BuildingManager.API.Domain.Entities;

public class Participant
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    [Required]
    public int UserId { get; set; }

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
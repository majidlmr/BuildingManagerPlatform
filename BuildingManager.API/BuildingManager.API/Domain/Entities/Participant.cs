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
}
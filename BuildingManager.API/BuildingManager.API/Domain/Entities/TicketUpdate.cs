// File: Domain/Entities/TicketUpdate.cs
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

public class TicketUpdate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } // Navigation Property

    [Required]
    public int UpdateByUserId { get; set; } // Soft-link to User
    public User UpdateBy { get; set; } // Navigation Property

    [Required]
    public string Comment { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
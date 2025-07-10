using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManager.API.Domain.Entities
{
    /// <summary>
    /// Represents an attachment for a ticket.
    /// (سند نیازمندی‌ها بخش ۷.۱)
    /// </summary>
    public class TicketAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; }

        [Required]
        [MaxLength(1024)] // Assuming a max URL length
        public string FileUrl { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UploadedByUserId { get; set; }
        [ForeignKey("UploadedByUserId")]
        public User UploadedByUser { get; set; }

        // Optional: Soft delete fields for consistency
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
        // public User? DeletedByUser { get; set; } // Navigation property for DeletedByUserId
    }
}

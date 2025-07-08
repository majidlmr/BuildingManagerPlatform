// File: Domain/Entities/Message.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

public class Message
{
    [Key]
    public int Id { get; set; }

    // 🚀 فیلد جدید: شناسه عمومی برای استفاده در API ها
    // این فیلد به ما اجازه می‌دهد تا شناسه اصلی (Id) را از کلاینت مخفی نگه داریم.
    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }

    [Required]
    public int SenderUserId { get; set; }
    public User Sender { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsAnonymous { get; set; } = false;

    [Required]
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }
}
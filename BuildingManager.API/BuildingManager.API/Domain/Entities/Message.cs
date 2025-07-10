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

    // Soft delete fields for the message entity itself
    public bool IsSoftDeleted { get; set; } = false;
    public DateTime? SoftDeletedAt { get; set; }
    public int? SoftDeletedByUserId { get; set; }
    // public User? SoftDeletedByUser { get; set; }

    // Note: The original IsDeleted and DeletedAt might have been intended for "deleted for everyone" logic.
    // If specific "delete for sender" or "delete for everyone" flags are needed beyond soft delete,
    // they would be separate properties. For now, standard soft delete is implemented.
}
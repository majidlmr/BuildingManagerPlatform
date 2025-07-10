// File: Domain/Entities/Conversation.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک رشته گفتگوی کامل در سیستم چت است.
/// </summary>
public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } // نوع گفتگو، مثلا "Direct" برای دو نفره یا "Group" برای گروهی

    /// <summary>
    /// 🚀 فیلد جدید: نام گفتگو.
    /// برای چت‌های دو نفره، نام طرف مقابل در اینجا ذخیره می‌شود.
    /// برای چت‌های گروهی، نام گروه در این فیلد قرار می‌گیرد.
    /// این فیلد واکشی لیست گفتگوها را بسیار بهینه می‌کند.
    /// </summary>
    [MaxLength(250)]
    public string? Name { get; set; }

    /// <summary>
    /// 🚀 فیلد جدید: آدرس تصویر گفتگو.
    /// برای چت‌های دو نفره، آدرس تصویر پروفایل طرف مقابل در اینجا ذخیره می‌شود.
    /// </summary>
    public string? ImageUrl { get; set; }

    // شناسه ساختمان، فقط برای چت‌های گروهی مربوط به یک ساختمان خاص کاربرد دارد
    public int? BlockId { get; set; } // Changed from BuildingId
    public Block Block { get; set; } // Changed from Building

    // لیست شرکت‌کنندگان در این گفتگو
    public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>(); // Changed from Participant

    // لیست پیام‌های ارسال شده در این گفتگو
    public ICollection<Message> Messages { get; set; } = new List<Message>();

    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    // public User? DeletedByUser { get; set; }
}
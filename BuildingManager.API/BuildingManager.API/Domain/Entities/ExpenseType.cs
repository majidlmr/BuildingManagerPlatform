using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities
{
    public enum DefaultPayer
    {
        Owner,          // هزینه معمولاً به عهده مالک است
        Tenant,         // هزینه معمولاً به عهده مستاجر است
        BuildingFund    // هزینه معمولاً از محل صندوق ساختمان پرداخت می‌شود
    }

    /// <summary>
    /// Represents a type or category of expense in the system.
    /// (سند نیازمندی‌ها بخش ۶.۴)
    /// </summary>
    public class ExpenseType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DefaultPayer DefaultPayer { get; set; }

        // Soft delete fields (اختیاری، در سند به صراحت برای این موجودیت ذکر نشده ولی برای هماهنگی می‌تواند اضافه شود)
        // public bool IsDeleted { get; set; } = false;
        // public DateTime? DeletedAt { get; set; }
        // public int? DeletedByUserId { get; set; }
    }
}

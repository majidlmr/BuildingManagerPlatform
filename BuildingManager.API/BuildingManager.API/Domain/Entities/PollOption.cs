using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManager.API.Domain.Entities;

/// <summary>
/// نمایانگر یک گزینه (Option) در یک نظرسنجی است.
/// </summary>
public class PollOption
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PollId { get; set; }
    public Poll Poll { get; set; }

    [Required]
    [MaxLength(250)]
    public string Text { get; set; } // متن گزینه

    /// <summary>
    /// لیستی از رای‌هایی که به این گزینه داده شده است.
    /// </summary>
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
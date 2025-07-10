using System;
using System.Collections.Generic;
using BuildingManager.API.Domain.Enums; // Added for Enums

namespace BuildingManager.API.Application.Features.Billing.Queries.GetInvoiceDetails;

/// <summary>
/// مدل نمایش جزئیات کامل یک صورتحساب به همراه ردیف‌های آن.
/// </summary>
public record InvoiceDetailsDto
{
    public Guid PublicId { get; init; }
    public Guid? ComplexPublicId { get; init; } // Added
    public Guid BlockPublicId { get; init; } // Added (assuming BlockId in Invoice maps to a Block with PublicId)
    public Guid UnitPublicId { get; init; } // Added (assuming UnitId in Invoice maps to a Unit with PublicId)
    public Guid UserPublicId { get; init; } // Added (User who is billed)
    public string Description { get; init; }
    public decimal Amount { get; init; } // Renamed from TotalAmount
    public InvoiceStatus Status { get; init; } // Changed to Enum
    public InvoiceType InvoiceType { get; init; } // Added
    public DateTime IssueDate { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? PaymentDate { get; init; } // Added
    public DateTime CreatedAt { get; init; } // Added
    public string BilledTo { get; init; } // نام کاربری که صورتحساب برای اوست (می‌تواند از UserPublicId واکشی شود)
    public List<InvoiceItemDto> Items { get; init; }
}

/// <summary>
/// مدل نمایش یک ردیف (آیتم) از صورتحساب.
/// </summary>
public record InvoiceItemDto(
    string Description,
    decimal Amount,
    // Consider if InvoiceItemType enum (if created) should be used here
    string Type // "شارژ ثابت" یا "سهم از هزینه"
);
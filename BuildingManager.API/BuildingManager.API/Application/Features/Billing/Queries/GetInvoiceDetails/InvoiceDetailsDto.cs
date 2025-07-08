using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Billing.Queries.GetInvoiceDetails;

/// <summary>
/// مدل نمایش جزئیات کامل یک صورتحساب به همراه ردیف‌های آن.
/// </summary>
public record InvoiceDetailsDto
{
    public Guid PublicId { get; init; }
    public string Description { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; }
    public DateTime IssueDate { get; init; }
    public DateTime DueDate { get; init; }
    public string BilledTo { get; init; } // نام کاربری که صورتحساب برای اوست
    public List<InvoiceItemDto> Items { get; init; }
}

/// <summary>
/// مدل نمایش یک ردیف (آیتم) از صورتحساب.
/// </summary>
public record InvoiceItemDto(
    string Description,
    decimal Amount,
    string Type // "شارژ ثابت" یا "سهم از هزینه"
);
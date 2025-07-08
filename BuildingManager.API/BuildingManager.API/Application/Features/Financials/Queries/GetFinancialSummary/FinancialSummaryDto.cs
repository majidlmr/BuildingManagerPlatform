using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Financials.Queries.GetFinancialSummary;

public record FinancialSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}

public record TransactionDto(
    string Title,
    string Type, // "درآمد" یا "هزینه"
    decimal Amount,
    DateTime Date,
    string Category
);
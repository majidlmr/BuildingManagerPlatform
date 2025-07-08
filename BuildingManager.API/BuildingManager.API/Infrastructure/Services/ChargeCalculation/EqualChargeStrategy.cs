using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingManager.API.Infrastructure.Services.ChargeCalculation;

/// <summary>
/// استراتژی تقسیم مساوی هزینه‌ها بین تمام واحدها.
/// </summary>
public class EqualChargeStrategy : IChargeCalculationStrategy
{
    public string Name => "Equal"; // نام استراتژی

    public Dictionary<int, List<InvoiceItem>> CalculateDues(IEnumerable<Expense> expenses, IEnumerable<Unit> units)
    {
        var dues = new Dictionary<int, List<InvoiceItem>>();
        if (!units.Any()) return dues;

        var totalUnits = units.Count();

        foreach (var expense in expenses)
        {
            if (expense.Amount == 0) continue;

            // محاسبه سهم هر واحد از این هزینه
            var sharePerUnit = Math.Round(expense.Amount / totalUnits, 2);

            // افزودن سهم به صورتحساب هر واحد
            foreach (var unit in units)
            {
                if (!dues.ContainsKey(unit.Id))
                {
                    dues[unit.Id] = new List<InvoiceItem>();
                }
                dues[unit.Id].Add(new InvoiceItem
                {
                    Type = InvoiceItemType.ExpenseShare,
                    Description = $"سهم از هزینه: {expense.Title}",
                    Amount = sharePerUnit
                });
            }
        }
        return dues;
    }
}
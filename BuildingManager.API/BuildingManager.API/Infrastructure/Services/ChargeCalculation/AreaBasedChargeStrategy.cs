using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingManager.API.Infrastructure.Services.ChargeCalculation;

/// <summary>
/// استراتژی تقسیم هزینه‌ها بر اساس متراژ واحدها.
/// </summary>
public class AreaBasedChargeStrategy : IChargeCalculationStrategy
{
    public string Name => "ByArea"; // نام استراتژی

    public Dictionary<int, List<InvoiceItem>> CalculateDues(IEnumerable<Expense> expenses, IEnumerable<Unit> units)
    {
        var dues = new Dictionary<int, List<InvoiceItem>>();
        if (!units.Any()) return dues;

        var totalArea = units.Sum(u => u.Area ?? 0);
        if (totalArea == 0) return dues; // اگر متراژ کل صفر باشد، از تقسیم بر صفر جلوگیری می‌کنیم

        foreach (var expense in expenses)
        {
            if (expense.Amount == 0) continue;

            foreach (var unit in units)
            {
                var unitArea = unit.Area ?? 0;
                if (unitArea == 0) continue;

                // محاسبه سهم هر واحد بر اساس متراژ
                var sharePerUnit = Math.Round((expense.Amount / totalArea) * unitArea, 2);

                if (!dues.ContainsKey(unit.Id))
                {
                    dues[unit.Id] = new List<InvoiceItem>();
                }
                dues[unit.Id].Add(new InvoiceItem
                {
                    Type = InvoiceItemType.ExpenseShare,
                    Description = $"سهم از هزینه (بر اساس متراژ): {expense.Title}",
                    Amount = sharePerUnit
                });
            }
        }
        return dues;
    }
}
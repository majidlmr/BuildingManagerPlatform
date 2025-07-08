using System;

namespace BuildingManager.API.Application.Features.Billing.Commands.CreateCycle;

/// <summary>
/// مدل ورودی برای ایجاد یک چرخه حسابداری جدید از طریق API.
/// </summary>
/// <param name="Name">عنوان چرخه (مثلاً: شارژ خرداد ۱۴۰۴).</param>
/// <param name="StartDate">تاریخ شروع دوره.</param>
/// <param name="EndDate">تاریخ پایان دوره و سررسید پرداخت.</param>
/// <param name="DefaultChargePerUnit">مبلغ شارژ ثابت برای هر واحد.</param>
public record CreateBillingCycleRequest(
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal DefaultChargePerUnit
);
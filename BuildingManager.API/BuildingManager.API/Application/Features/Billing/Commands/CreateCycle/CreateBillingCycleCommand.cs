using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Billing.Commands.CreateCycle;

/// <summary>
/// دستوری برای ایجاد یک چرخه حسابداری جدید و صدور خودکار صورتحساب‌ها.
/// </summary>
/// <param name="BuildingId">شناسه ساختمان.</param>
/// <param name="RequestingUserId">شناسه مدیر درخواست دهنده.</param>
/// <param name="Name">عنوان چرخه (مثلاً: شارژ خرداد ۱۴۰۴).</param>
/// <param name="StartDate">تاریخ شروع دوره.</param>
/// <param name="EndDate">تاریخ پایان دوره و سررسید پرداخت.</param>
/// <param name="DefaultChargePerUnit">مبلغ شارژ ثابت برای هر واحد.</param>
public record CreateBillingCycleCommand(
    int BuildingId,
    int RequestingUserId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    decimal DefaultChargePerUnit
) : IRequest<int>;
using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Polls.Commands.CreatePoll;

/// <summary>
/// مدلی که اطلاعات یک نظرسنجی جدید را از کلاینت (ورودی مدیر) دریافت می‌کند.
/// </summary>
/// <param name="Question">سوال اصلی نظرسنجی.</param>
/// <param name="Options">لیستی از متون گزینه‌های نظرسنجی.</param>
/// <param name="IsMultipleChoice">آیا کاربر می‌تواند چند گزینه را انتخاب کند؟</param>
/// <param name="EndDate">تاریخ پایان نظرسنجی (اختیاری).</param>
public record CreatePollRequest(
    string Question,
    List<string> Options,
    bool IsMultipleChoice,
    DateTime? EndDate
);
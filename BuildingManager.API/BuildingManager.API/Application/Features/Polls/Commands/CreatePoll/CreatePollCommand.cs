using MediatR;
using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Polls.Commands.CreatePoll;

/// <summary>
/// دستوری برای ایجاد یک نظرسنجی جدید در یک ساختمان مشخص.
/// </summary>
/// <param name="BuildingId">شناسه ساختمان.</param>
/// <param name="Question">سوال اصلی نظرسنجی.</param>
/// <param name="Options">لیست گزینه‌ها.</param>
/// <param name="IsMultipleChoice">امکان انتخاب چند گزینه.</param>
/// <param name="EndDate">تاریخ پایان (اختیاری).</param>
/// <param name="CreatedByUserId">شناسه کاربری که نظرسنجی را ایجاد کرده.</param>
public record CreatePollCommand(
    int BuildingId,
    string Question,
    List<string> Options,
    bool IsMultipleChoice,
    DateTime? EndDate,
    int CreatedByUserId
) : IRequest<int>; // شناسه نظرسنجی جدید را برمی‌گرداند
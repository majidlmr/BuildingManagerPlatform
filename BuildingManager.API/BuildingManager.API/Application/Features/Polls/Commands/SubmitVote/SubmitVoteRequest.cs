using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Polls.Commands.SubmitVote;

/// <summary>
/// مدلی که رای کاربر را از کلاینت دریافت می‌کند.
/// </summary>
/// <param name="OptionIds">لیستی از شناسه‌های گزینه‌هایی که کاربر به آن‌ها رای داده است.</param>
public record SubmitVoteRequest(List<int> OptionIds);
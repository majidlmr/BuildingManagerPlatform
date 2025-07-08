using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Polls.Commands.SubmitVote;

/// <summary>
/// دستوری برای ثبت رای یک کاربر در یک نظرسنجی.
/// </summary>
/// <param name="PollId">شناسه نظرسنجی.</param>
/// <param name="OptionIds">شناسه‌های گزینه‌های انتخابی.</param>
/// <param name="UserId">شناسه کاربری که رای می‌دهد.</param>
public record SubmitVoteCommand(
    int PollId,
    List<int> OptionIds,
    int UserId
) : IRequest; // این دستور نتیجه خاصی برنمی‌گرداند
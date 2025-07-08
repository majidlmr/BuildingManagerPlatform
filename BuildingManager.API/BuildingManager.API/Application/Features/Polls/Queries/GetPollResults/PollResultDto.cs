using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Polls.Queries.GetPollResults;

/// <summary>
/// مدل نمایش نتایج کامل یک نظرسنجی.
/// </summary>
public record PollResultDto
{
    public string Question { get; init; }
    public int TotalVotes { get; init; }
    public List<PollOptionResultDto> Options { get; init; }
}

/// <summary>
/// مدل نمایش نتایج برای یک گزینه خاص.
/// </summary>
public record PollOptionResultDto(
    string Text,
    int VoteCount,
    double Percentage
);
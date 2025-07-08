using BuildingManager.API.Application.Features.Polls.Commands.CreatePoll;
using BuildingManager.API.Application.Features.Polls.Commands.SubmitVote;
using BuildingManager.API.Application.Features.Polls.Queries.GetPollResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingManager.API.Domain.Constants;

namespace BuildingManager.API.Controllers;

/// <summary>
/// کنترلری برای مدیریت نظرسنجی‌ها و رای‌گیری‌های ساختمان.
/// </summary>
[ApiController]
[Route("api/buildings/{buildingId}/polls")]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PollsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// یک نظرسنجی جدید برای ساختمان مشخص شده ایجاد می‌کند. (فقط برای مدیر ساختمان)
    /// </summary>
    /// <param name="buildingId">شناسه ساختمان.</param>
    /// <param name="request">اطلاعات نظرسنجی جدید.</param>
    /// <returns>شناسه نظرسنجی ایجاد شده.</returns>
    [HttpPost]
    [Authorize(Roles = UserRoles.BuildingManager)]
    public async Task<IActionResult> CreatePoll(int buildingId, [FromBody] CreatePollRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreatePollCommand(
            buildingId,
            request.Question,
            request.Options,
            request.IsMultipleChoice,
            request.EndDate,
            userId
        );

        var pollId = await _mediator.Send(command);

        return StatusCode(201, new { PollId = pollId });
    }

    /// <summary>
    /// رای کاربر را برای یک نظرسنجی خاص ثبت می‌کند.
    /// </summary>
    /// <param name="buildingId">شناسه ساختمان (برای مسیر).</param>
    /// <param name="pollId">شناسه نظرسنجی.</param>
    /// <param name="request">شناسه‌های گزینه‌های انتخابی.</param>
    [HttpPost("{pollId}/vote")]
    public async Task<IActionResult> SubmitVote(int buildingId, int pollId, [FromBody] SubmitVoteRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // buildingId در اینجا برای سازگاری با مسیر است، اما در command اصلی استفاده نمی‌شود
        var command = new SubmitVoteCommand(pollId, request.OptionIds, userId);

        await _mediator.Send(command);

        return Ok(new { message = "رای شما با موفقیت ثبت شد." });
    }

    /// <summary>
    /// نتایج یک نظرسنجی را نمایش می‌دهد.
    /// </summary>
    /// <param name="pollId">شناسه نظرسنجی.</param>
    [HttpGet("{pollId}/results")]
    public async Task<IActionResult> GetPollResults(int pollId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetPollResultsQuery(pollId, userId);

        var results = await _mediator.Send(query);

        return Ok(results);
    }
}
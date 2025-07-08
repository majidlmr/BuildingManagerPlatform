using BuildingManager.API.Application.Features.Announcements.Commands.CreateAnnouncement;
using BuildingManager.API.Application.Features.Announcements.Queries.GetAnnouncements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api/buildings/{buildingId}/announcements")]
[Authorize]
public class AnnouncementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnnouncementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "BuildingManager")]
    public async Task<IActionResult> CreateAnnouncement(int buildingId, [FromBody] CreateAnnouncementRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateAnnouncementCommand(buildingId, request.Title, request.Content, request.ExpiresAt, request.IsPinned, userId);

        var announcementId = await _mediator.Send(command);

        return StatusCode(201, new { AnnouncementId = announcementId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAnnouncements(int buildingId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetAnnouncementsQuery(buildingId, userId);

        var announcements = await _mediator.Send(query);

        return Ok(announcements);
    }
}

// مدل درخواست برای ایجاد اعلان
public record CreateAnnouncementRequest(string Title, string Content, DateTime? ExpiresAt, bool IsPinned);
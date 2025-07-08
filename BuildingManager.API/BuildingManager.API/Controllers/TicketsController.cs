// File: Controllers/TicketsController.cs
using BuildingManager.API.Application.Features.Tickets.Commands.AddComment;
using BuildingManager.API.Application.Features.Tickets.Commands.CreateTicket;
using BuildingManager.API.Application.Features.Tickets.Commands.UpdateStatus;
using BuildingManager.API.Application.Features.Tickets.Queries.GetBuildingTickets;
using BuildingManager.API.Application.Features.Tickets.Queries.GetTicketDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using BuildingManager.API.Domain.Constants;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// یک تیکت جدید برای یک ساختمان مشخص ثبت می‌کند.
    /// </summary>
    [HttpPost("buildings/{buildingId}/tickets")]
    public async Task<IActionResult> CreateTicket(int buildingId, [FromBody] CreateTicketRequest request)
    {
        var reportedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 🚀 تغییر اصلی: پارامتر جدید IsAnonymous را به Command پاس می‌دهیم
        var command = new CreateTicketCommand(
            buildingId,
            request.UnitId,
            reportedByUserId,
            request.Title,
            request.Description,
            request.Category,
            request.Priority,
            request.AttachmentUrl,
            request.IsAnonymous // ✅ این پارامتر جدید به Command اضافه شد
        );

        var ticketPublicId = await _mediator.Send(command);
        return StatusCode(201, new { TicketId = ticketPublicId });
    }

    /// <summary>
    /// لیست تمام تیکت‌های مربوط به یک ساختمان را برمی‌گرداند.
    /// </summary>
    [HttpGet("buildings/{buildingId}/tickets")]
    public async Task<IActionResult> GetTicketsForBuilding(int buildingId)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetBuildingTicketsQuery(buildingId, requestingUserId);
        var tickets = await _mediator.Send(query);
        return Ok(tickets);
    }

    /// <summary>
    /// جزئیات یک تیکت خاص را بر اساس شناسه عمومی آن برمی‌گرداند.
    /// </summary>
    [HttpGet("tickets/{id}")]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetTicketDetailsQuery(id, requestingUserId);
        var ticket = await _mediator.Send(query);
        return Ok(ticket);
    }

    /// <summary>
    /// وضعیت یک تیکت خاص را به‌روزرسانی می‌کند. (فقط برای مدیر ساختمان)
    /// </summary>
    [HttpPatch("tickets/{id}/status")]
    [Authorize(Roles = UserRoles.BuildingManager)]
    public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] UpdateTicketStatusRequest request)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new UpdateTicketStatusCommand(id, request.NewStatus, requestingUserId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// یک کامنت جدید به تیکت اضافه می‌کند.
    /// </summary>
    [HttpPost("tickets/{id}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new AddCommentCommand(id, request.Comment, userId);
        var commentId = await _mediator.Send(command);
        return StatusCode(201, new { CommentId = commentId });
    }
}
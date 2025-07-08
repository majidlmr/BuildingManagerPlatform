using BuildingManager.API.Application.Features.Residents.Commands.AssignResident;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingManager.API.Domain.Constants;

namespace BuildingManager.API.Controllers;

// درخواست جدید که تاریخ شروع و پایان را هم دریافت می‌کند
public record AssignResidentRequest(
    string ResidentPhoneNumber,
    string ResidentFullName,
    DateTime StartDate,
    DateTime? EndDate
);

[ApiController]
[Route("api/units")]
[Authorize(Roles = UserRoles.BuildingManager)]
public class UnitsController : ControllerBase
{
    private readonly IMediator _mediator;
    public UnitsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("{unitId}/assign-resident")]
    public async Task<IActionResult> AssignResident(int unitId, [FromBody] AssignResidentRequest request)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ارسال تمام پارامترهای جدید به Command
        var command = new AssignResidentCommand(
            unitId,
            request.ResidentPhoneNumber,
            request.ResidentFullName,
            request.StartDate,
            request.EndDate,
            requestingUserId
        );

        var assignmentId = await _mediator.Send(command);
        return Ok(new { AssignmentId = assignmentId });
    }
}
using BuildingManager.API.Application.Features.Vehicles.Commands.CreateVehicle;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

/// <summary>
/// کنترلری برای مدیریت وسایل نقلیه کاربران.
/// </summary>
[ApiController]
[Route("api/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// یک وسیله نقلیه جدید برای کاربر لاگین کرده، ثبت می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateVehicleCommand(userId, request.LicensePlate, request.Model, request.Color, request.Description);

        var vehicleId = await _mediator.Send(command);

        return StatusCode(201, new { VehicleId = vehicleId });
    }
}
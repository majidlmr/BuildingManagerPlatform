using BuildingManager.API.Application.Features.Buildings.Commands.Create;
using BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingDetails;
using BuildingManager.API.Application.Features.Buildings.Queries.GetMyBuildings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

/// <summary>
/// کنترلری برای مدیریت ساختمان‌ها و مجتمع‌ها.
/// </summary>
[ApiController]
[Route("api/buildings")]
[Authorize] // تمام عملیات‌ها نیاز به کاربر احراز هویت شده دارند
public class BuildingsController : ControllerBase
{
    private readonly IMediator _mediator;
    public BuildingsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// لیست تمام ساختمان‌هایی را که کاربر فعلی در آن‌ها نقش مدیریتی دارد، برمی‌گرداند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyBuildings()
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetMyBuildingsQuery(ownerId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// جزئیات یک ساختمان یا بلوک خاص را برمی‌گرداند.
    /// (دسترسی در لایه Application بررسی می‌شود)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBuildingById(int id)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetBuildingDetailsQuery(id, requestingUserId);
        var building = await _mediator.Send(query);
        return Ok(building);
    }

    /// <summary>
    /// یک ساختمان یا مجتمع جدید (سطح بالا) ایجاد می‌کند.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBuilding([FromBody] CreateBuildingRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateBuildingCommand(
            request.Name, request.BuildingType, request.Address, request.NumberOfFloors,
            request.TotalUnits, request.ConstructionYear, request.Latitude,
            request.Longitude, request.Amenities, ownerId);

        var buildingId = await _mediator.Send(command);
        return StatusCode(201, new { BuildingId = buildingId });
    }

    /// <summary>
    /// یک بلوک (ساختمان) جدید به عنوان زیرمجموعه یک مجتمع ایجاد می‌کند.
    /// (نیاز به دسترسی برای مدیریت مجتمع والد دارد)
    /// </summary>
    /// <param name="parentBuildingId">شناسه مجتمعی که بلوک به آن اضافه می‌شود.</param>
    /// <param name="request">اطلاعات بلوک جدید.</param>
    [HttpPost("{parentBuildingId:int}/blocks")]
    public async Task<IActionResult> CreateBlock(int parentBuildingId, [FromBody] CreateBuildingRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // از همان Command قبلی استفاده می‌کنیم، اما این بار ParentBuildingId را نیز ارسال می‌کنیم
        var command = new CreateBuildingCommand(
            request.Name, request.BuildingType, request.Address, request.NumberOfFloors,
            request.TotalUnits, request.ConstructionYear, request.Latitude,
            request.Longitude, request.Amenities, ownerId, parentBuildingId); // ✅ ارسال شناسه والد

        var blockId = await _mediator.Send(command);
        return StatusCode(201, new { BlockId = blockId });
    }
}
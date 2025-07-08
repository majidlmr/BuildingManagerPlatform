using BuildingManager.API.Application.Features.Permissions.Queries.GetAllPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PermissionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// لیست تمام دسترسی‌های قابل تعریف در کل سیستم را برمی‌گرداند.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _mediator.Send(new GetAllPermissionsQuery());
        return Ok(permissions);
    }
}
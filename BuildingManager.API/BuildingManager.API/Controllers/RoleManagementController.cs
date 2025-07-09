using BuildingManager.API.Application.Features.RoleManagement.Commands.AssignPermission;
using BuildingManager.API.Application.Features.RoleManagement.Queries.GetRolePermissions;
// TODO: Add using statements for Role CRUD commands/queries when created
using MediatR;
using Microsoft.AspNetCore.Authorization; // For later use
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic; // For List

namespace BuildingManager.API.Controllers
{
    [ApiController]
    [Route("api/role-management")]
    // [Authorize(Roles="SuperAdmin")] // Example: Only SuperAdmin can manage roles and permissions
    public class RoleManagementController : ControllerBase
    {
        private readonly ISender _mediator;

        public RoleManagementController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("roles/{roleNormalizedName}/assign-permission")]
        // [Authorize(Policy = "CanAssignPermissionsToRole")]
        public async Task<IActionResult> AssignPermissionToRole(string roleNormalizedName, [FromBody] AssignPermissionToRoleCommand command)
        {
            if (roleNormalizedName != command.RoleNormalizedName)
            {
                return BadRequest("RoleNormalizedName in route does not match RoleNormalizedName in command body.");
            }
            // command.AssignedByUserId = ... // Get from current authenticated user
            var success = await _mediator.Send(command);
            return success ? Ok(new { Message = "مجوز با موفقیت به نقش تخصیص داده شد." })
                           : BadRequest(new { Message = "خطا در تخصیص مجوز به نقش. بررسی کنید نقش و مجوز معتبر باشند و تخصیص تکراری نباشد." });
        }

        [HttpGet("roles/{roleNormalizedName}/permissions")]
        // [Authorize(Policy = "CanViewRolePermissions")]
        public async Task<ActionResult<List<PermissionResponseDto>>> GetRolePermissions(string roleNormalizedName)
        {
            var query = new GetRolePermissionsQuery { RoleNormalizedName = roleNormalizedName };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // TODO: Add Endpoints for Role CRUD (Create, GetAll, GetById, Update, Delete)
        // These will require their own Commands/Queries and Handlers.
        // Example:
        // [HttpPost("roles")]
        // public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command) { ... }

        // [HttpGet("roles")]
        // public async Task<IActionResult> GetAllRoles([FromQuery] GetAllRolesQuery query) { ... }

        // TODO: Add Endpoints for Permission CRUD (primarily for SuperAdmin to manage the list of available permissions)
        // Example:
        // [HttpGet("permissions")]
        // public async Task<IActionResult> GetAllPermissions([FromQuery] GetAllPermissionsQuery query) { ... }
    }
}

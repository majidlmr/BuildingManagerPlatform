using BuildingManager.API.Application.Features.UserManagement.Commands.AssignRole;
using BuildingManager.API.Application.Features.UserManagement.Queries.GetUserRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization; // For later use
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic; // For List

namespace BuildingManager.API.Controllers
{
    [ApiController]
    [Route("api/user-management")]
    // [Authorize] // Secure this controller appropriately
    public class UserManagementController : ControllerBase
    {
        private readonly ISender _mediator;

        public UserManagementController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("assign-role")]
        // [Authorize(Policy = "CanAssignRoles")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand command)
        {
            // In a real app, AssignedByUserId would come from the authenticated user's claims
            // For now, if it's part of the command, ensure it's validated or set by a trusted source.
            // command.AssignedByUserId = GetCurrentUserId();

            var success = await _mediator.Send(command);
            return success ? Ok(new { Message = "نقش با موفقیت به کاربر تخصیص داده شد." })
                           : BadRequest(new { Message = "خطا در تخصیص نقش به کاربر." });
        }

        [HttpGet("users/{userPublicId:guid}/roles")]
        // [Authorize(Policy = "CanViewUserRoles")]
        public async Task<ActionResult<List<UserRoleResponseDto>>> GetUserRoles(Guid userPublicId, [FromQuery] Guid? targetEntityPublicId, [FromQuery] Domain.Entities.HierarchyLevel? targetEntityType)
        {
            var query = new GetUserRolesQuery
            {
                UserPublicId = userPublicId,
                TargetEntityPublicId = targetEntityPublicId,
                TargetEntityType = targetEntityType
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

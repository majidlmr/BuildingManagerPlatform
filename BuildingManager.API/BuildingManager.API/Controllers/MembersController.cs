using BuildingManager.API.Application.Features.Members.Queries.GetBuildingMembers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingManager.API.Application.Features.Members.Commands.RemoveUserFromRole;
using BuildingManager.API.Application.Features.Members.Commands.RemoveMemberFromBuilding;

namespace BuildingManager.API.Controllers;

/// <summary>
/// کنترلری برای مدیریت اعضا و نقش‌های آن‌ها در یک ساختمان.
/// </summary>
[ApiController]
[Route("api/buildings/{buildingId}/members")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// یک عضو را به همراه تمام نقش‌هایش از ساختمان حذف می‌کند.
    /// (نیاز به دسترسی Member.Remove دارد)
    /// </summary>
    /// <param name="buildingId">شناسه ساختمان.</param>
    /// <param name="userId">شناسه کاربری که باید حذف شود.</param>
    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> RemoveMemberFromBuilding(int buildingId, int userId)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new RemoveMemberFromBuildingCommand(buildingId, userId, requestingUserId);
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// یک نقش مشخص را از یک کاربر در ساختمان حذف می‌کند.
    /// (نیاز به دسترسی Member.Remove دارد)
    /// </summary>
    /// <param name="buildingId">شناسه ساختمان.</param>
    /// <param name="userId">شناسه کاربری که نقش از او حذف می‌شود.</param>
    /// <param name="roleId">شناسه نقشی که باید حذف شود.</param>
    [HttpDelete("{userId:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveUserFromRole(int buildingId, int userId, int roleId)
    {
        var requestingUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new RemoveUserFromRoleCommand(buildingId, userId, roleId, requestingUserId);
        await _mediator.Send(command);

        return NoContent(); // کد 204 No Content نشان‌دهنده موفقیت عملیات حذف است
    }

    /// <summary>
    /// لیست تمام کاربرانی که در این ساختمان نقش دارند را به همراه نقش‌هایشان برمی‌گرداند.
    /// (نیاز به دسترسی Member.Read دارد)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBuildingMembers(int buildingId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetBuildingMembersQuery(buildingId, userId);
        var members = await _mediator.Send(query);
        return Ok(members);
    }
}
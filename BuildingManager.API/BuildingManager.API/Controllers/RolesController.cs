using BuildingManager.API.Application.Features.Roles.Commands.AssignPermissionToRole;
using BuildingManager.API.Application.Features.Roles.Commands.AssignUserToRole;
using BuildingManager.API.Application.Features.Roles.Commands.CreateRole;
using BuildingManager.API.Application.Features.Roles.Commands.DeleteRole;
using BuildingManager.API.Application.Features.Roles.Commands.UpdateRole;
using BuildingManager.API.Application.Features.Roles.Queries.GetBuildingRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BuildingManager.API.Controllers;

/// <summary>
/// کنترلری برای مدیریت کامل نقش‌ها (ایجاد، مشاهده، ویرایش، حذف) و دسترسی‌ها در یک ساختمان.
/// </summary>
[ApiController]
[Route("api/buildings/{buildingId}/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// یک نقش جدید برای ساختمان مشخص شده ایجاد می‌کند. (نیاز به دسترسی Role.Create)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRole(int buildingId, [FromBody] CreateRoleRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateRoleCommand(buildingId, request.RoleName, userId);
        var roleId = await _mediator.Send(command);

        return StatusCode(201, new { RoleId = roleId });
    }

    /// <summary>
    /// لیستی از تمام نقش‌های تعریف شده برای یک ساختمان را برمی‌گرداند.
    /// (نیاز به عضویت در ساختمان دارد)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBuildingRoles(int buildingId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetBuildingRolesQuery(buildingId, userId);
        var roles = await _mediator.Send(query);
        return Ok(roles);
    }

    /// <summary>
    /// نام یک نقش موجود را ویرایش می‌کند. (نیاز به دسترسی Role.Update)
    /// </summary>
    [HttpPut("{roleId:int}")]
    public async Task<IActionResult> UpdateRole(int buildingId, int roleId, [FromBody] UpdateRoleRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new UpdateRoleCommand(buildingId, roleId, request.NewName, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// یک نقش را به طور کامل حذف می‌کند. (نیاز به دسترسی Role.Delete)
    /// </summary>
    [HttpDelete("{roleId:int}")]
    public async Task<IActionResult> DeleteRole(int buildingId, int roleId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new DeleteRoleCommand(buildingId, roleId, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// دسترسی‌های مشخصی را به یک نقش موجود تخصیص می‌دهد. (نیاز به دسترسی Role.Assign)
    /// </summary>
    [HttpPost("{roleId:int}/permissions")]
    public async Task<IActionResult> AssignPermissionsToRole(int buildingId, int roleId, [FromBody] AssignPermissionsRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new AssignPermissionToRoleCommand(roleId, request.PermissionIds, userId, buildingId);
        await _mediator.Send(command);
        return Ok(new { message = "دسترسی‌ها با موفقیت به نقش تخصیص داده شد." });
    }

    /// <summary>
    /// یک کاربر مشخص را به یک نقش در ساختمان اضافه می‌کند. (نیاز به دسترسی Role.Assign)
    /// </summary>
    [HttpPost("{roleId:int}/users")]
    public async Task<IActionResult> AssignUserToRole(int buildingId, int roleId, [FromBody] AssignUserToRoleRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new AssignUserToRoleCommand(buildingId, roleId, request.UserIdToAssign, userId);
        await _mediator.Send(command);
        return Ok(new { message = "کاربر با موفقیت به نقش مورد نظر اضافه شد." });
    }
}

// مدل‌های ورودی از API برای کنترلر
public record CreateRoleRequest(string RoleName);
public record UpdateRoleRequest(string NewName);
public record AssignPermissionsRequest(List<int> PermissionIds);
public record AssignUserToRoleRequest(int UserIdToAssign);
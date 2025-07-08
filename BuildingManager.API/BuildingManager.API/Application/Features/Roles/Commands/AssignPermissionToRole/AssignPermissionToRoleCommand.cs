using MediatR;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Features.Roles.Commands.AssignPermissionToRole;

/// <summary>
/// دستوری برای تخصیص یک یا چند دسترسی به یک نقش مشخص.
/// </summary>
/// <param name="RoleId">شناسه نقشی که دسترسی‌ها به آن اضافه می‌شود.</param>
/// <param name="PermissionIds">لیستی از شناسه‌های دسترسی‌هایی که باید تخصیص داده شوند.</param>
/// <param name="RequestingUserId">شناسه کاربری که درخواست را داده است.</param>
/// <param name="BuildingId">شناسه ساختمانی که عملیات در آن انجام می‌شود (برای بررسی دسترسی).</param>
public record AssignPermissionToRoleCommand(
    int RoleId,
    List<int> PermissionIds,
    int RequestingUserId,
    int BuildingId
) : IRequest;
using MediatR;

namespace BuildingManager.API.Application.Features.Members.Commands.RemoveUserFromRole;

/// <summary>
/// دستوری برای حذف یک نقش خاص از یک کاربر در یک ساختمان.
/// </summary>
/// <param name="BuildingId">شناسه ساختمان (برای بررسی دسترسی).</param>
/// <param name="UserIdToRemove">شناسه کاربری که نقش از او حذف می‌شود.</param>
/// <param name="RoleId">شناسه نقشی که باید حذف شود.</param>
/// <param name="RequestingUserId">شناسه کاربری که درخواست را داده است.</param>
public record RemoveUserFromRoleCommand(
    int BuildingId,
    int UserIdToRemove,
    int RoleId,
    int RequestingUserId
) : IRequest;
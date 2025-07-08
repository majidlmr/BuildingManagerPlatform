using MediatR;

namespace BuildingManager.API.Application.Features.Roles.Commands.CreateRole;

/// <summary>
/// دستوری برای ایجاد یک نقش جدید در یک ساختمان مشخص.
/// </summary>
/// <param name="BuildingId">شناسه ساختمانی که نقش در آن ایجاد می‌شود.</param>
/// <param name="RoleName">نام نقش جدید (مثلا: "حسابدار").</param>
/// <param name="RequestingUserId">شناسه کاربری که درخواست را داده است.</param>
public record CreateRoleCommand(int BuildingId, string RoleName, int RequestingUserId) : IRequest<int>;
using MediatR;

namespace BuildingManager.API.Application.Features.Members.Commands.RemoveMemberFromBuilding;

/// <summary>
/// دستوری برای حذف کامل یک عضو (و تمام نقش‌هایش) از یک ساختمان مشخص.
/// </summary>
/// <param name="BuildingId">شناسه ساختمان.</param>
/// <param name="MemberUserId">شناسه کاربری که باید از ساختمان حذف شود.</param>
/// <param name="RequestingUserId">شناسه کاربری که درخواست را داده است.</param>
public record RemoveMemberFromBuildingCommand(
    int BuildingId,
    int MemberUserId,
    int RequestingUserId
) : IRequest;
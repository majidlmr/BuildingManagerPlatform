// File: Application/Features/Buildings/Queries/GetBuildingDetails/GetBuildingDetailsQueryHandler.cs
using BuildingManager.API.Application.Common.Exceptions; // ✅ ۱. افزودن using برای خطاهای سفارشی
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingDetails;

public class GetBuildingDetailsQueryHandler : IRequestHandler<GetBuildingDetailsQuery, BuildingDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService; // ✅ ۲. افزودن سرویس احراز هویت

    // ✅ ۳. تزریق سرویس احراز هویت در سازنده کلاس
    public GetBuildingDetailsQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService;
    }

    public async Task<BuildingDetailsDto> Handle(GetBuildingDetailsQuery request, CancellationToken cancellationToken)
    {
        // ✅ ۴. بررسی امنیتی: آیا کاربر درخواست‌دهنده، عضو این ساختمان است؟
        var canAccess = await _authorizationService.IsMemberOfBuildingAsync(request.RequestingUserId, request.BuildingId, cancellationToken);
        if (!canAccess)
        {
            // 🚀 پرتاب خطای مشخص "عدم دسترسی" که کد 403 را برمی‌گرداند
            throw new ForbiddenAccessException();
        }

        var building = await _context.Buildings
            .Include(b => b.Units)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BuildingId, cancellationToken);

        if (building == null)
        {
            // 🚀 پرتاب خطای مشخص "یافت نشد" که کد 404 را برمی‌گرداند
            throw new NotFoundException(nameof(Building), request.BuildingId);
        }

        // بقیه منطق شما بدون تغییر باقی می‌ماند
        var buildingDto = new BuildingDetailsDto(
            building.PublicId,
            building.Name,
            building.BuildingType,
            building.Address,
            building.NumberOfFloors,
            building.TotalUnits,
            building.Latitude,
            building.Longitude,
            building.Amenities,
            building.Units.Select(u => new UnitDto(
                u.PublicId,
                u.UnitNumber,
                u.UnitType,
                u.FloorNumber,
                u.Bedrooms,
                u.Area,
                u.OwnershipStatus
            )).ToList()
        );

        return buildingDto;
    }
}
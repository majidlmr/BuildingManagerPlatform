using BuildingManager.API.Application.Common.Exceptions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq; // 👈 using جدید
using System.Threading; // 👈 using جدید
using System.Threading.Tasks; // 👈 using جدید

namespace BuildingManager.API.Application.Features.Buildings.Queries.GetBuildingWithUnits;

public class GetBuildingWithUnitsQueryHandler : IRequestHandler<GetBuildingWithUnitsQuery, BuildingDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService; // 👈 سرویس دسترسی

    public GetBuildingWithUnitsQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
    {
        _context = context;
        _authorizationService = authorizationService; // 👈 مقداردهی
    }

    public async Task<BuildingDetailsDto> Handle(GetBuildingWithUnitsQuery request, CancellationToken cancellationToken)
    {
        // ✅ بررسی امنیتی: آیا کاربر درخواست‌دهنده، عضو این ساختمان است؟
        var canAccess = await _authorizationService.IsMemberOfBuildingAsync(request.RequestingUserId, request.BuildingId, cancellationToken);
        if (!canAccess)
        {
            throw new ForbiddenAccessException("شما اجازه دسترسی به اطلاعات این ساختمان را ندارید.");
        }

        var building = await _context.Buildings
            .Include(b => b.Units)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BuildingId, cancellationToken);

        if (building == null)
        {
            throw new NotFoundException(nameof(Building), request.BuildingId);
        }

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
                u.Area
            )).ToList()
        );

        return buildingDto;
    }
}
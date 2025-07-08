using BuildingManager.API.Domain.Interfaces;  
using MediatR;
using System.Collections.Generic;  
using System.Linq;                
using System.Threading;          
using System.Threading.Tasks;     

namespace BuildingManager.API.Application.Features.Buildings.Queries.GetMyBuildings;

public class GetMyBuildingsQueryHandler : IRequestHandler<GetMyBuildingsQuery, List<BuildingSummaryDto>>
{
    private readonly IBuildingRepository _buildingRepository; // ✅ استفاده از ریپازیتوری به جای DbContext مستقیم

    public GetMyBuildingsQueryHandler(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public async Task<List<BuildingSummaryDto>> Handle(GetMyBuildingsQuery request, CancellationToken cancellationToken)
    {
        // ✅ فراخوانی متد جدید از ریپازیتوری
        var buildings = await _buildingRepository.GetBuildingsByManagerIdAsync(request.OwnerUserId);

        // Map کردن نتیجه به DTO
        return buildings.Select(b => new BuildingSummaryDto(
                b.Id,
                b.PublicId,
                b.Name,
                b.BuildingType,
                b.Address
            )).ToList();
    }
}
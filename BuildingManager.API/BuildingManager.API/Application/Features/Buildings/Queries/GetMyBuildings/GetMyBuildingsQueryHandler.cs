using BuildingManager.API.Domain.Interfaces;  
using MediatR;
using System.Collections.Generic;  
using System.Linq;                
using System.Threading;          
using System.Threading.Tasks;     

namespace BuildingManager.API.Application.Features.Buildings.Queries.GetMyBuildings;

public class GetMyBuildingsQueryHandler : IRequestHandler<GetMyBuildingsQuery, List<BuildingSummaryDto>>
{
    private readonly IBlockRepository _blockRepository; // Changed from IBuildingRepository

    public GetMyBuildingsQueryHandler(IBlockRepository blockRepository) // Changed from IBuildingRepository
    {
        _blockRepository = blockRepository;
    }

    public async Task<List<BuildingSummaryDto>> Handle(GetMyBuildingsQuery request, CancellationToken cancellationToken)
    {
        // Call the renamed method from IBlockRepository
        var blocks = await _blockRepository.GetBlocksByManagerIdAsync(request.OwnerUserId); // Changed method name and variable

        // Map the result to DTO
        // Ensure BuildingSummaryDto can be constructed with Block properties
        // or consider creating a BlockSummaryDto
        return blocks.Select(b => new BuildingSummaryDto(
                b.Id,
                b.PublicId,
                b.NameOrNumber, // Assuming Block entity uses NameOrNumber
                b.BlockType,    // Assuming Block entity uses BlockType
                b.Address       // Assuming Block entity has Address
            )).ToList();
    }
}
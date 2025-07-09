// using AutoMapper;
// using AutoMapper.QueryableExtensions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Features.Blocks.Queries.GetBlockById; // For BlockResponseDto
using BuildingManager.API.Domain.Entities; // For Block entity for direct querying
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Blocks.Queries.GetAllBlocks
{
    public class GetAllBlocksQueryHandler : IRequestHandler<GetAllBlocksQuery, GetAllBlocksResponse>
    {
        private readonly IApplicationDbContext _context;
        // private readonly IMapper _mapper;

        public GetAllBlocksQueryHandler(IApplicationDbContext context /*, IMapper mapper*/)
        {
            _context = context;
            // _mapper = mapper;
        }

        public async Task<GetAllBlocksResponse> Handle(GetAllBlocksQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Block> query = _context.Blocks
                                          .AsNoTracking()
                                          .Include(b => b.ParentComplex) // Include parent for ParentComplexName/PublicId
                                          .Where(b => !b.IsDeleted);

            if (request.ParentComplexPublicId.HasValue)
            {
                // Find the internal Id of the complex first
                var parentComplexId = await _context.Complexes
                                            .AsNoTracking()
                                            .Where(c => c.PublicId == request.ParentComplexPublicId.Value && !c.IsDeleted)
                                            .Select(c => c.Id)
                                            .FirstOrDefaultAsync(cancellationToken);
                if(parentComplexId > 0)
                {
                    query = query.Where(b => b.ParentComplexId == parentComplexId);
                }
                else
                {
                    // If ParentComplexPublicId is provided but not found, return empty list
                    return new GetAllBlocksResponse { Items = new List<BlockResponseDto>() };
                }
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTermLower = request.SearchTerm.ToLower();
                query = query.Where(b =>
                    (b.NameOrNumber != null && b.NameOrNumber.ToLower().Contains(searchTermLower)) ||
                    (b.Address != null && b.Address.ToLower().Contains(searchTermLower))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var blocks = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Manual mapping
            var blockDtos = blocks.Select(block => new BlockResponseDto
            {
                Id = block.Id,
                PublicId = block.PublicId,
                ParentComplexPublicId = block.ParentComplex?.PublicId,
                ParentComplexName = block.ParentComplex?.Name,
                NameOrNumber = block.NameOrNumber,
                BlockType = block.BlockType,
                NumberOfFloors = block.NumberOfFloors,
                TotalUnits = block.TotalUnits,
                ConstructionYear = block.ConstructionYear,
                Address = block.Address,
                Latitude = block.Latitude,
                Longitude = block.Longitude,
                Amenities = block.Amenities,
                ChargeCalculationStrategyName = block.ChargeCalculationStrategyName,
                RulesFileUrl = block.RulesFileUrl,
                CreatedAt = block.CreatedAt,
                UpdatedAt = block.UpdatedAt
            }).ToList();

            // With AutoMapper:
            // var blockDtos = await query
            //     .OrderByDescending(b => b.CreatedAt)
            //     .Skip((request.PageNumber - 1) * request.PageSize)
            //     .Take(request.PageSize)
            //     .ProjectTo<BlockResponseDto>(_mapper.ConfigurationProvider)
            //     .ToListAsync(cancellationToken);


            return new GetAllBlocksResponse
            {
                Items = blockDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}

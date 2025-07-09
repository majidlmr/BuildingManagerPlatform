// using AutoMapper;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Blocks.Queries.GetBlockById
{
    public class GetBlockByIdQueryHandler : IRequestHandler<GetBlockByIdQuery, BlockResponseDto?>
    {
        private readonly IApplicationDbContext _context;
        // private readonly IMapper _mapper;

        public GetBlockByIdQueryHandler(IApplicationDbContext context /*, IMapper mapper*/)
        {
            _context = context;
            // _mapper = mapper;
        }

        public async Task<BlockResponseDto?> Handle(GetBlockByIdQuery request, CancellationToken cancellationToken)
        {
            var block = await _context.Blocks
                .AsNoTracking()
                .Include(b => b.ParentComplex) // Include parent complex to get its PublicId and Name
                .FirstOrDefaultAsync(b => b.PublicId == request.PublicId && !b.IsDeleted, cancellationToken);

            if (block == null)
            {
                return null;
            }

            // Manual mapping
            return new BlockResponseDto
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
            };

            // With AutoMapper, you'd configure a mapping from Block to BlockResponseDto
            // return _mapper.Map<BlockResponseDto>(block);
        }
    }
}

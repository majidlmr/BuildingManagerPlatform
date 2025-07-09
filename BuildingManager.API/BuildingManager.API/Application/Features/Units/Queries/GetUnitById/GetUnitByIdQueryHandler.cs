// using AutoMapper;
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Units.Queries.GetUnitById
{
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, UnitResponseDto?>
    {
        private readonly IApplicationDbContext _context;
        // private readonly IMapper _mapper;

        public GetUnitByIdQueryHandler(IApplicationDbContext context /*, IMapper mapper*/)
        {
            _context = context;
            // _mapper = mapper;
        }

        public async Task<UnitResponseDto?> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            var unit = await _context.Units
                .AsNoTracking()
                .Include(u => u.Block) // Include Block for BlockPublicId and Name
                .FirstOrDefaultAsync(u => u.PublicId == request.PublicId && !u.IsDeleted, cancellationToken);

            if (unit == null)
            {
                return null;
            }

            // Manual mapping
            return new UnitResponseDto
            {
                Id = unit.Id,
                PublicId = unit.PublicId,
                BlockPublicId = unit.Block.PublicId, // Assuming Block is not null due to FK constraint
                BlockNameOrNumber = unit.Block.NameOrNumber,
                UnitNumber = unit.UnitNumber,
                FloorNumber = unit.FloorNumber,
                Area = unit.Area,
                UnitType = unit.UnitType,
                NumberOfBedrooms = unit.NumberOfBedrooms,
                Description = unit.Description,
                CreatedAt = unit.CreatedAt,
                UpdatedAt = unit.UpdatedAt
                // Add CurrentAssignments mapping here if you implement that in DTO
            };

            // With AutoMapper:
            // return _mapper.Map<UnitResponseDto>(unit);
        }
    }
}

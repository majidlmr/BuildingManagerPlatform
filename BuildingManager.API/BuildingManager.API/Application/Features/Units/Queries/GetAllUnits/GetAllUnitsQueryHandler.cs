// using AutoMapper;
// using AutoMapper.QueryableExtensions;
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Features.Units.Queries.GetUnitById; // For UnitResponseDto
using BuildingManager.API.Domain.Entities; // For Unit entity
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Units.Queries.GetAllUnits
{
    public class GetAllUnitsQueryHandler : IRequestHandler<GetAllUnitsQuery, GetAllUnitsResponse>
    {
        private readonly IApplicationDbContext _context;
        // private readonly IMapper _mapper;

        public GetAllUnitsQueryHandler(IApplicationDbContext context /*, IMapper mapper*/)
        {
            _context = context;
            // _mapper = mapper;
        }

        public async Task<GetAllUnitsResponse> Handle(GetAllUnitsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Unit> query = _context.Units
                                        .AsNoTracking()
                                        .Include(u => u.Block) // To get BlockPublicId and Name
                                        .Where(u => !u.IsDeleted);

            if (request.BlockPublicId.HasValue)
            {
                 var blockId = await _context.Blocks
                                            .AsNoTracking()
                                            .Where(b => b.PublicId == request.BlockPublicId.Value && !b.IsDeleted)
                                            .Select(b => b.Id)
                                            .FirstOrDefaultAsync(cancellationToken);
                if(blockId > 0)
                {
                    query = query.Where(u => u.BlockId == blockId);
                }
                else
                {
                    // If BlockPublicId is provided but not found, return empty list
                    return new GetAllUnitsResponse { Items = new List<UnitResponseDto>() };
                }
            }
            else
            {
                // Depending on requirements, you might want to enforce BlockPublicId
                // or allow fetching all units across all blocks (potentially a large dataset).
                // For now, let's assume if no BlockPublicId, it's an invalid request or fetches nothing.
                // To fetch all units, remove this else block or modify query accordingly.
                 return new GetAllUnitsResponse { Items = new List<UnitResponseDto>(), Message = "BlockPublicId is required." };
            }


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTermLower = request.SearchTerm.ToLower();
                query = query.Where(u =>
                    (u.UnitNumber != null && u.UnitNumber.ToLower().Contains(searchTermLower)) ||
                    (u.Description != null && u.Description.ToLower().Contains(searchTermLower))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var units = await query
                .OrderBy(u => u.FloorNumber).ThenBy(u => u.UnitNumber) // Example ordering
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Manual mapping
            var unitDtos = units.Select(unit => new UnitResponseDto
            {
                Id = unit.Id,
                PublicId = unit.PublicId,
                BlockPublicId = unit.Block.PublicId,
                BlockNameOrNumber = unit.Block.NameOrNumber,
                UnitNumber = unit.UnitNumber,
                FloorNumber = unit.FloorNumber,
                Area = unit.Area,
                UnitType = unit.UnitType,
                NumberOfBedrooms = unit.NumberOfBedrooms,
                Description = unit.Description,
                CreatedAt = unit.CreatedAt,
                UpdatedAt = unit.UpdatedAt
            }).ToList();

            // With AutoMapper:
            // var unitDtos = await query
            //     .OrderBy(u => u.FloorNumber).ThenBy(u => u.UnitNumber)
            //     .Skip((request.PageNumber - 1) * request.PageSize)
            //     .Take(request.PageSize)
            //     .ProjectTo<UnitResponseDto>(_mapper.ConfigurationProvider)
            //     .ToListAsync(cancellationToken);

            return new GetAllUnitsResponse
            {
                Items = unitDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}

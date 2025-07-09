// using AutoMapper; // If using AutoMapper
// using AutoMapper.QueryableExtensions; // For ProjectTo with AutoMapper
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById; // For ComplexResponseDto
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Complexes.Queries.GetAllComplexes
{
    public class GetAllComplexesQueryHandler : IRequestHandler<GetAllComplexesQuery, GetAllComplexesResponse>
    {
        private readonly IApplicationDbContext _context;
        // private readonly IMapper _mapper; // Inject IMapper if you set it up

        public GetAllComplexesQueryHandler(IApplicationDbContext context /*, IMapper mapper*/)
        {
            _context = context;
            // _mapper = mapper;
        }

        public async Task<GetAllComplexesResponse> Handle(GetAllComplexesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Complexes.AsNoTracking().Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTermLower = request.SearchTerm.ToLower();
                query = query.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(searchTermLower)) ||
                    (c.Address != null && c.Address.ToLower().Contains(searchTermLower))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt) // Default ordering
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(complex => new ComplexResponseDto // Manual mapping
                {
                    Id = complex.Id,
                    PublicId = complex.PublicId,
                    Name = complex.Name,
                    Address = complex.Address,
                    ComplexType = complex.ComplexType,
                    Latitude = complex.Latitude,
                    Longitude = complex.Longitude,
                    Amenities = complex.Amenities,
                    RulesFileUrl = complex.RulesFileUrl,
                    CreatedAt = complex.CreatedAt,
                    UpdatedAt = complex.UpdatedAt
                })
                // With AutoMapper: .ProjectTo<ComplexResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new GetAllComplexesResponse
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}

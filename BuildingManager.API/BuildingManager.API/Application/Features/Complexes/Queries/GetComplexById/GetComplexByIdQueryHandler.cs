using AutoMapper; // Assuming AutoMapper is or will be used for DTO mapping
using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById
{
    public class GetComplexByIdQueryHandler : IRequestHandler<GetComplexByIdQuery, ComplexResponseDto?>
    {
        private readonly IApplicationDbContext _context;
        // private readonly IMapper _mapper; // Inject IMapper if you set it up

        // Constructor without IMapper for now
        public GetComplexByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        // Constructor with IMapper (if you decide to use AutoMapper)
        // public GetComplexByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        // {
        //     _context = context;
        //     _mapper = mapper;
        // }

        public async Task<ComplexResponseDto?> Handle(GetComplexByIdQuery request, CancellationToken cancellationToken)
        {
            var complex = await _context.Complexes
                .AsNoTracking() // Good practice for read-only queries
                .FirstOrDefaultAsync(c => c.PublicId == request.PublicId && !c.IsDeleted, cancellationToken);

            if (complex == null)
            {
                return null;
            }

            // Manual mapping for now, replace with AutoMapper if available
            return new ComplexResponseDto
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
            };

            // With AutoMapper:
            // return _mapper.Map<ComplexResponseDto>(complex);
        }
    }
}

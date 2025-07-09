using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Complexes.Commands.CreateComplex
{
    public class CreateComplexCommandHandler : IRequestHandler<CreateComplexCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateComplexCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateComplexCommand request, CancellationToken cancellationToken)
        {
            var complex = new Complex
            {
                Name = request.Name,
                Address = request.Address,
                ComplexType = request.ComplexType,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Amenities = request.Amenities,
                RulesFileUrl = request.RulesFileUrl,
                CreatedAt = DateTime.UtcNow,
                PublicId = Guid.NewGuid() // Ensure PublicId is set
            };

            await _context.Complexes.AddAsync(complex, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return complex.PublicId;
        }
    }
}

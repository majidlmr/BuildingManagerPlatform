using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingManager.API.Application.Common.Exceptions; // For NotFoundException

namespace BuildingManager.API.Application.Features.Complexes.Commands.UpdateComplex
{
    public class UpdateComplexCommandHandler : IRequestHandler<UpdateComplexCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateComplexCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateComplexCommand request, CancellationToken cancellationToken)
        {
            var complex = await _context.Complexes
                .FirstOrDefaultAsync(c => c.PublicId == request.PublicId && !c.IsDeleted, cancellationToken);

            if (complex == null)
            {
                // Or throw a NotFoundException if you have a middleware to handle it
                // throw new NotFoundException(nameof(Complex), request.PublicId);
                return false;
            }

            complex.Name = request.Name;
            complex.Address = request.Address;
            complex.ComplexType = request.ComplexType;
            complex.Latitude = request.Latitude;
            complex.Longitude = request.Longitude;
            complex.Amenities = request.Amenities;
            complex.RulesFileUrl = request.RulesFileUrl;
            complex.UpdatedAt = DateTime.UtcNow;

            // _context.Complexes.Update(complex); // Not always necessary if tracking is enabled
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

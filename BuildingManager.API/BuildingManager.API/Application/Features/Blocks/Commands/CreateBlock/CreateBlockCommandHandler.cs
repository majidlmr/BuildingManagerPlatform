using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // For FirstOrDefaultAsync

namespace BuildingManager.API.Application.Features.Blocks.Commands.CreateBlock
{
    public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateBlockCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
        {
            int? parentComplexId = null;
            if (request.ParentComplexPublicId.HasValue)
            {
                var parentComplex = await _context.Complexes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.PublicId == request.ParentComplexPublicId.Value && !c.IsDeleted, cancellationToken);

                if (parentComplex == null)
                {
                    throw new ApplicationException($"مجتمع با شناسه {request.ParentComplexPublicId} یافت نشد."); // Or a specific NotFoundException
                }
                parentComplexId = parentComplex.Id;
            }

            var block = new Block
            {
                PublicId = Guid.NewGuid(),
                ParentComplexId = parentComplexId,
                NameOrNumber = request.NameOrNumber,
                BlockType = request.BlockType,
                NumberOfFloors = request.NumberOfFloors,
                TotalUnits = request.TotalUnits,
                ConstructionYear = request.ConstructionYear,
                Address = request.Address, // If null, it might inherit from complex or be standalone
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Amenities = request.Amenities,
                ChargeCalculationStrategyName = request.ChargeCalculationStrategyName,
                RulesFileUrl = request.RulesFileUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Blocks.AddAsync(block, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return block.PublicId;
        }
    }
}

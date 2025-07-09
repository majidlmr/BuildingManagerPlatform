using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Common.Exceptions; // For NotFoundException
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Blocks.Commands.UpdateBlock
{
    public class UpdateBlockCommandHandler : IRequestHandler<UpdateBlockCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateBlockCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateBlockCommand request, CancellationToken cancellationToken)
        {
            var block = await _context.Blocks
                .FirstOrDefaultAsync(b => b.PublicId == request.PublicId && !b.IsDeleted, cancellationToken);

            if (block == null)
            {
                // throw new NotFoundException(nameof(Block), request.PublicId);
                return false;
            }

            // Handle ParentComplexId update
            if (request.ParentComplexPublicId.HasValue)
            {
                var parentComplex = await _context.Complexes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.PublicId == request.ParentComplexPublicId.Value && !c.IsDeleted, cancellationToken);
                if (parentComplex == null)
                {
                    throw new ApplicationException($"مجتمع والد با شناسه {request.ParentComplexPublicId} یافت نشد.");
                }
                block.ParentComplexId = parentComplex.Id;
            }
            else
            {
                block.ParentComplexId = null; // Make it a standalone block
            }

            block.NameOrNumber = request.NameOrNumber;
            block.BlockType = request.BlockType;
            block.NumberOfFloors = request.NumberOfFloors;
            block.TotalUnits = request.TotalUnits;
            block.ConstructionYear = request.ConstructionYear;
            block.Address = request.Address;
            block.Latitude = request.Latitude;
            block.Longitude = request.Longitude;
            block.Amenities = request.Amenities;
            block.ChargeCalculationStrategyName = request.ChargeCalculationStrategyName;
            block.RulesFileUrl = request.RulesFileUrl;
            block.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

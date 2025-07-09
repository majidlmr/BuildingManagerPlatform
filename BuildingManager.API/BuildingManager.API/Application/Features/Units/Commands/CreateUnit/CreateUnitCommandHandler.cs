using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Common.Exceptions; // For NotFoundException
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Units.Commands.CreateUnit
{
    public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            var block = await _context.Blocks
                .FirstOrDefaultAsync(b => b.PublicId == request.BlockPublicId && !b.IsDeleted, cancellationToken);

            if (block == null)
            {
                throw new NotFoundException($"بلوک با شناسه {request.BlockPublicId} یافت نشد.", nameof(Block));
            }

            // Optional: Check if UnitNumber already exists in this block
            var unitExists = await _context.Units
                .AnyAsync(u => u.BlockId == block.Id && u.UnitNumber == request.UnitNumber && !u.IsDeleted, cancellationToken);
            if (unitExists)
            {
                // Consider a more specific exception like ConflictException
                throw new ApplicationException($"واحد با شماره '{request.UnitNumber}' در این بلوک قبلا ثبت شده است.");
            }

            var unit = new Unit
            {
                PublicId = Guid.NewGuid(),
                BlockId = block.Id,
                UnitNumber = request.UnitNumber,
                FloorNumber = request.FloorNumber,
                Area = request.Area,
                UnitType = request.UnitType,
                NumberOfBedrooms = request.NumberOfBedrooms,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Units.AddAsync(unit, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return unit.PublicId;
        }
    }
}

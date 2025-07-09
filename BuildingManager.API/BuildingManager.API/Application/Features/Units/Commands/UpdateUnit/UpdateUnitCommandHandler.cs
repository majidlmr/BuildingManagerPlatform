using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Application.Common.Exceptions; // For NotFoundException
using BuildingManager.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingManager.API.Application.Features.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.PublicId == request.PublicId && !u.IsDeleted, cancellationToken);

            if (unit == null)
            {
                // throw new NotFoundException(nameof(Unit), request.PublicId);
                return false;
            }

            // Check if UnitNumber is being changed and if the new one already exists in this block
            if (unit.UnitNumber != request.UnitNumber)
            {
                var unitExists = await _context.Units
                    .AnyAsync(u => u.BlockId == unit.BlockId && u.UnitNumber == request.UnitNumber && u.PublicId != request.PublicId && !u.IsDeleted, cancellationToken);
                if (unitExists)
                {
                    throw new ApplicationException($"واحد با شماره '{request.UnitNumber}' در این بلوک قبلا ثبت شده است.");
                }
            }

            // Note: Changing unit.BlockId is a more complex operation (moving a unit to another block)
            // and should typically be handled by a dedicated command or process if allowed.
            // We are not allowing BlockId to be changed in UpdateUnitCommand for now.

            unit.UnitNumber = request.UnitNumber;
            unit.FloorNumber = request.FloorNumber;
            unit.Area = request.Area;
            unit.UnitType = request.UnitType;
            unit.NumberOfBedrooms = request.NumberOfBedrooms;
            unit.Description = request.Description;
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

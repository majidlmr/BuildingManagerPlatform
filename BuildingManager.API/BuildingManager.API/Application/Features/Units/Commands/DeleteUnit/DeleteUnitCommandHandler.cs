using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
// using BuildingManager.API.Application.Common.Exceptions;

namespace BuildingManager.API.Application.Features.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteUnitCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var unit = await _context.Units
                .FirstOrDefaultAsync(u => u.PublicId == request.PublicId && !u.IsDeleted, cancellationToken);

            if (unit == null)
            {
                // throw new NotFoundException(nameof(Unit), request.PublicId);
                return false;
            }

            // Business rule: Check if there are any active (non-ended, non-cancelled) UnitAssignments.
            // A unit with active residents/owners should probably not be deleted directly.
            // This depends on the desired behavior:
            // 1. Prevent deletion.
            // 2. Soft-delete assignments as well (cascade soft delete).
            // 3. End/Cancel active assignments.
            // For now, let's prevent deletion if active assignments exist.
            var hasActiveAssignments = await _context.UnitAssignments
                .AnyAsync(ua => ua.UnitId == unit.Id &&
                                !ua.IsDeleted &&
                                (ua.AssignmentStatus == Domain.Entities.UnitAssignmentStatus.Active ||
                                 ua.AssignmentStatus == Domain.Entities.UnitAssignmentStatus.Future),
                                cancellationToken);

            if (hasActiveAssignments)
            {
                // throw new DeletionForbiddenException("Cannot delete unit with active or future assignments. Please end or cancel assignments first.");
                return false; // Or throw an exception
            }

            // Also consider other dependencies: unpaid invoices, open tickets specific to this unit, etc.
            // For MVP, we'll keep it simpler.

            unit.IsDeleted = true;
            unit.DeletedAt = DateTime.UtcNow;
            unit.DeletedByUserId = request.DeletedByUserId;
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

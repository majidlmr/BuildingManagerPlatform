using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
// For NotFoundException if you choose to use it:
// using BuildingManager.API.Application.Common.Exceptions;

namespace BuildingManager.API.Application.Features.Blocks.Commands.DeleteBlock
{
    public class DeleteBlockCommandHandler : IRequestHandler<DeleteBlockCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteBlockCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteBlockCommand request, CancellationToken cancellationToken)
        {
            var block = await _context.Blocks
                .FirstOrDefaultAsync(b => b.PublicId == request.PublicId && !b.IsDeleted, cancellationToken);

            if (block == null)
            {
                // throw new NotFoundException(nameof(Block), request.PublicId);
                return false;
            }

            // Check if there are any active (non-soft-deleted) Units associated with this Block.
            var hasActiveUnits = await _context.Units.AnyAsync(u => u.BlockId == block.Id && !u.IsDeleted, cancellationToken);
            if (hasActiveUnits)
            {
                // Cannot delete block if it has active units.
                // Consider a more descriptive response/exception.
                // throw new DeletionForbiddenException("Cannot delete block with active units. Please delete or reassign units first.");
                return false;
            }

            block.IsDeleted = true;
            block.DeletedAt = DateTime.UtcNow;
            block.DeletedByUserId = request.DeletedByUserId; // Will be set from authenticated user context
            block.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

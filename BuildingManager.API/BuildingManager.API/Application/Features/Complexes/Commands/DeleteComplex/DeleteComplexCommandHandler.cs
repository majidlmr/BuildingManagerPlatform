using BuildingManager.API.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingManager.API.Application.Common.Exceptions; // For NotFoundException

namespace BuildingManager.API.Application.Features.Complexes.Commands.DeleteComplex
{
    public class DeleteComplexCommandHandler : IRequestHandler<DeleteComplexCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteComplexCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteComplexCommand request, CancellationToken cancellationToken)
        {
            var complex = await _context.Complexes
                .FirstOrDefaultAsync(c => c.PublicId == request.PublicId && !c.IsDeleted, cancellationToken);

            if (complex == null)
            {
                // Or throw a NotFoundException
                return false;
            }

            // Check if there are any active (non-soft-deleted) Blocks associated with this Complex.
            // Depending on business rules, you might want to prevent deletion or handle cascading soft deletes.
            // For now, we'll assume we only soft-delete the complex itself.
            // Cascading soft delete logic would be more complex and involve updating child entities.
            var hasActiveBlocks = await _context.Blocks.AnyAsync(b => b.ParentComplexId == complex.Id && !b.IsDeleted, cancellationToken);
            if (hasActiveBlocks)
            {
                // Cannot delete complex if it has active blocks.
                // You could return a specific error message or throw a custom exception.
                // For simplicity, returning false. Consider a more descriptive response in a real app.
                // throw new DeletionForbiddenException("Cannot delete complex with active blocks. Please delete or reassign blocks first.");
                return false; // Or throw an exception that the controller can catch
            }


            complex.IsDeleted = true;
            complex.DeletedAt = DateTime.UtcNow;
            complex.DeletedByUserId = request.DeletedByUserId; // Make sure to get this from authenticated user context later
            complex.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

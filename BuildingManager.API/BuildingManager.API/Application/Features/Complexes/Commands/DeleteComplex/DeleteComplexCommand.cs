using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Complexes.Commands.DeleteComplex
{
    public class DeleteComplexCommand : IRequest<bool> // Returns true if soft delete was successful
    {
        public Guid PublicId { get; set; }
        public int? DeletedByUserId { get; set; } // Optional: to record who performed the delete
    }
}

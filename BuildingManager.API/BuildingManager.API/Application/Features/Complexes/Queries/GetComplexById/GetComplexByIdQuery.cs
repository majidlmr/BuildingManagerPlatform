using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById
{
    public class GetComplexByIdQuery : IRequest<ComplexResponseDto?>
    {
        public Guid PublicId { get; set; }
    }
}

using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Units.Queries.GetUnitById
{
    public class GetUnitByIdQuery : IRequest<UnitResponseDto?>
    {
        public Guid PublicId { get; set; }
    }
}

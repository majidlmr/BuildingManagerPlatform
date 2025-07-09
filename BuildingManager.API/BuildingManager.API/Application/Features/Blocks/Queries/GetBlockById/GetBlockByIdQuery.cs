using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Blocks.Queries.GetBlockById
{
    public class GetBlockByIdQuery : IRequest<BlockResponseDto?>
    {
        public Guid PublicId { get; set; }
    }
}

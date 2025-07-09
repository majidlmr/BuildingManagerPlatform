using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Blocks.Commands.DeleteBlock
{
    public class DeleteBlockCommand : IRequest<bool>
    {
        public Guid PublicId { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}

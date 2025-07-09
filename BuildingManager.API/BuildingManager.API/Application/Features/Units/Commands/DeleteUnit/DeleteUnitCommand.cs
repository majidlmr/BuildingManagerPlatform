using MediatR;
using System;

namespace BuildingManager.API.Application.Features.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommand : IRequest<bool>
    {
        public Guid PublicId { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}

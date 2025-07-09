using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For UnitType enum

namespace BuildingManager.API.Application.Features.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<bool>
    {
        public Guid PublicId { get; set; } // To identify the unit
        // BlockPublicId is generally not updatable for a unit; if it needs to move, it's usually delete-recreate or a special "move" operation.
        // public Guid BlockPublicId { get; set; }
        public string UnitNumber { get; set; }
        public int? FloorNumber { get; set; }
        public decimal Area { get; set; }
        public UnitType UnitType { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public string? Description { get; set; }
    }
}

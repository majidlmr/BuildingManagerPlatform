using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For UnitType enum

namespace BuildingManager.API.Application.Features.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<Guid> // Returns PublicId of the created unit
    {
        public Guid BlockPublicId { get; set; } // Parent Block's PublicId
        public string UnitNumber { get; set; }
        public int? FloorNumber { get; set; }
        public decimal Area { get; set; }
        public UnitType UnitType { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public string? Description { get; set; }
    }
}

using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For BlockType enum

namespace BuildingManager.API.Application.Features.Blocks.Commands.UpdateBlock
{
    public class UpdateBlockCommand : IRequest<bool>
    {
        public Guid PublicId { get; set; } // To identify the block
        public Guid? ParentComplexPublicId { get; set; } // Can be null to make it standalone, or set to a new complex
        public string NameOrNumber { get; set; }
        public BlockType BlockType { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? TotalUnits { get; set; }
        public int? ConstructionYear { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Amenities { get; set; }
        public string ChargeCalculationStrategyName { get; set; }
        public string? RulesFileUrl { get; set; }
    }
}

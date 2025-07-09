using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For BlockType enum

namespace BuildingManager.API.Application.Features.Blocks.Commands.CreateBlock
{
    public class CreateBlockCommand : IRequest<Guid> // Returns PublicId of the created block
    {
        public Guid? ParentComplexPublicId { get; set; } // Optional: If this block belongs to a complex
        public string NameOrNumber { get; set; }
        public BlockType BlockType { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? TotalUnits { get; set; }
        public int? ConstructionYear { get; set; }
        public string? Address { get; set; } // Can be same as complex or specific
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Amenities { get; set; }
        public string ChargeCalculationStrategyName { get; set; } = "Equal"; // Default
        public string? RulesFileUrl { get; set; }
    }
}

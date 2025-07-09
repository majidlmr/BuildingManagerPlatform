using System;
using BuildingManager.API.Domain.Entities; // For BlockType enum

namespace BuildingManager.API.Application.Features.Blocks.Queries.GetBlockById
{
    public class BlockResponseDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public Guid? ParentComplexPublicId { get; set; } // PublicId of the parent Complex
        public string ParentComplexName { get; set; } // Optional: Name of the parent Complex for display
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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Consider adding a count of units or simplified unit DTOs if needed directly
    }
}

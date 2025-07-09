using System;
using BuildingManager.API.Domain.Entities; // For ComplexType enum

namespace BuildingManager.API.Application.Features.Complexes.Queries.GetComplexById
{
    public class ComplexResponseDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public ComplexType ComplexType { get; set; } // Assuming ComplexType is an enum
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Amenities { get; set; }
        public string? RulesFileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // We might want to add a list of Block IDs or simplified Block DTOs here in the future
        // public ICollection<int> BlockIds { get; set; } = new List<int>();
    }
}

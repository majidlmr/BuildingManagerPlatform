using System;
using BuildingManager.API.Domain.Entities; // For UnitType enum

namespace BuildingManager.API.Application.Features.Units.Queries.GetUnitById
{
    public class UnitResponseDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public Guid BlockPublicId { get; set; } // PublicId of the parent Block
        public string BlockNameOrNumber { get; set; } // Optional: Name/Number of the parent Block
        public string UnitNumber { get; set; }
        public int? FloorNumber { get; set; }
        public decimal Area { get; set; }
        public UnitType UnitType { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Information about current assignments (owner/tenant) can be added here if needed
        // For example:
        // public List<UnitAssignmentDto> CurrentAssignments { get; set; } = new List<UnitAssignmentDto>();
    }
}

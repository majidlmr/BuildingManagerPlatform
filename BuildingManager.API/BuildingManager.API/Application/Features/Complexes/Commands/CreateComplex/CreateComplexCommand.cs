using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For ComplexType enum

namespace BuildingManager.API.Application.Features.Complexes.Commands.CreateComplex
{
    public class CreateComplexCommand : IRequest<Guid> // Returns PublicId of the created complex
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public ComplexType ComplexType { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Amenities { get; set; } // Could be JSON
        public string? RulesFileUrl { get; set; }
    }
}

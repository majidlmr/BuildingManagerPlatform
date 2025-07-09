using MediatR;
using System;
using BuildingManager.API.Domain.Entities; // For ComplexType enum

namespace BuildingManager.API.Application.Features.Complexes.Commands.UpdateComplex
{
    public class UpdateComplexCommand : IRequest<bool> // Returns true if update was successful
    {
        public Guid PublicId { get; set; } // To identify the complex to update
        public string Name { get; set; }
        public string? Address { get; set; }
        public ComplexType ComplexType { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Amenities { get; set; }
        public string? RulesFileUrl { get; set; }
    }
}

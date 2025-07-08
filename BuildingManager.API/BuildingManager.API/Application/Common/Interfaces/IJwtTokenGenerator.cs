// File: Application/Common/Interfaces/IJwtTokenGenerator.cs
using BuildingManager.API.Domain.Entities;
namespace BuildingManager.API.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
using System;
using System.Collections.Generic;

namespace BuildingManager.API.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(int userId, Guid publicId, string firstName, string lastName, string phoneNumber, IReadOnlyList<string> roles);
    }
}
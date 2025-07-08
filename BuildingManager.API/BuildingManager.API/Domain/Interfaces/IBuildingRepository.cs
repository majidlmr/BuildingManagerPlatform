using BuildingManager.API.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BuildingManager.API.Domain.Interfaces;

public interface IBuildingRepository
{
    Task<Building?> GetByIdAsync(int id);
    Task<Building?> GetByIdWithUnitsAsync(int id);
    Task<IEnumerable<Building>> GetBuildingsByManagerIdAsync(int managerUserId);
    Task AddAsync(Building building);
}
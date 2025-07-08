using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Interfaces;

public interface IUnitRepository
{
    Task<Unit?> GetByIdWithBuildingAsync(int id);
    Task AddAsync(Unit unit);
}
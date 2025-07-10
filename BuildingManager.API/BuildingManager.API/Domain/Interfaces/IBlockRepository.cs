using BuildingManager.API.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BuildingManager.API.Domain.Interfaces;

public interface IBlockRepository // Renamed from IBuildingRepository
{
    Task<Block?> GetByIdAsync(int id); // Changed Building to Block
    Task<Block?> GetByIdWithUnitsAsync(int id); // Changed Building to Block
    Task<IEnumerable<Block>> GetBlocksByManagerIdAsync(int managerUserId); // Changed Building to Block and method name
    Task AddAsync(Block block); // Changed Building to Block
}
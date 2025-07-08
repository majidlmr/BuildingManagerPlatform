using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildingManager.API.Infrastructure.Persistence.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly IApplicationDbContext _context;
    public UnitRepository(IApplicationDbContext context) => _context = context;

    public async Task AddAsync(Unit unit) => await _context.Units.AddAsync(unit);

    public async Task<Unit?> GetByIdWithBuildingAsync(int id)
    {
        return await _context.Units
            .Include(u => u.Building)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
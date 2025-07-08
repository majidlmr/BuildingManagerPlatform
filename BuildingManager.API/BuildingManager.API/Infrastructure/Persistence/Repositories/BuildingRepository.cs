// File: Infrastructure/Persistence/Repositories/BuildingRepository.cs
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BuildingManager.API.Infrastructure.Persistence.Repositories;

public class BuildingRepository : IBuildingRepository
{
    private readonly IApplicationDbContext _context;

    public BuildingRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Building building)
    {
        await _context.Buildings.AddAsync(building);
    }

    // --- متد جدید و فراموش شده در اینجا اضافه می‌شود ---
    public async Task<Building?> GetByIdAsync(int id)
    {
        return await _context.Buildings.FindAsync(id);
    }
    // ---------------------------------------------

    public async Task<Building?> GetByIdWithUnitsAsync(int id)
    {
        return await _context.Buildings
            .Include(b => b.Units)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Building>> GetBuildingsByManagerIdAsync(int managerUserId)
    {
        return await _context.ManagerAssignments
            .Where(m => m.UserId == managerUserId)
            .Select(m => m.Building)
            .OrderByDescending(b => b.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}
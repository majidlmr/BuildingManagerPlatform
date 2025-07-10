// File: Infrastructure/Persistence/Repositories/BlockRepository.cs
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces; // Ensure this points to the correct namespace if IBlockRepository moved
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for Task

namespace BuildingManager.API.Infrastructure.Persistence.Repositories;

public class BlockRepository : IBlockRepository // Renamed from BuildingRepository, implements IBlockRepository
{
    private readonly IApplicationDbContext _context;

    public BlockRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Block block) // Changed Building to Block
    {
        await _context.Blocks.AddAsync(block); // Changed Buildings to Blocks
    }

    public async Task<Block?> GetByIdAsync(int id) // Changed Building to Block
    {
        return await _context.Blocks.FindAsync(id); // Changed Buildings to Blocks
    }

    public async Task<Block?> GetByIdWithUnitsAsync(int id) // Changed Building to Block
    {
        return await _context.Blocks // Changed Buildings to Blocks
            .Include(b => b.Units)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Block>> GetBlocksByManagerIdAsync(int managerUserId) // Changed Building to Block and method name
    {
        return await _context.ManagerAssignments
            .Where(m => m.UserId == managerUserId)
            .Select(m => m.Block) // Changed from Building to Block
            .OrderByDescending(b => b.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}
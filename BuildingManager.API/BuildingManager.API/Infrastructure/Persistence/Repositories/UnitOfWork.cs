// File: Infrastructure/Persistence/Repositories/UnitOfWork.cs
using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Interfaces;

namespace BuildingManager.API.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IApplicationDbContext _context;

    public UnitOfWork(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
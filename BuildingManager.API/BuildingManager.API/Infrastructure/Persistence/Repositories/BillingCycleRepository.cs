using BuildingManager.API.Application.Common.Interfaces;
using BuildingManager.API.Domain.Entities;
using BuildingManager.API.Domain.Interfaces;

namespace BuildingManager.API.Infrastructure.Persistence.Repositories;

public class BillingCycleRepository : IBillingCycleRepository
{
    private readonly IApplicationDbContext _context;
    public BillingCycleRepository(IApplicationDbContext context) => _context = context;

    public async Task AddAsync(BillingCycle billingCycle) => await _context.BillingCycles.AddAsync(billingCycle);
}
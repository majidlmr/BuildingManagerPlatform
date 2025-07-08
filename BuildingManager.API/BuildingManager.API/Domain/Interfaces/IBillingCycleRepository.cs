using BuildingManager.API.Domain.Entities;

namespace BuildingManager.API.Domain.Interfaces;

public interface IBillingCycleRepository
{
    Task AddAsync(BillingCycle billingCycle);
}
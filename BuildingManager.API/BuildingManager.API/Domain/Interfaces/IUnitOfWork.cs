// File: Domain/Interfaces/IUnitOfWork.cs
namespace BuildingManager.API.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
using BuildingManager.API.Domain.Entities;
using System.Threading.Tasks;

namespace BuildingManager.API.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber); // جستجو بر اساس شماره موبایل
    Task AddAsync(User user);
}
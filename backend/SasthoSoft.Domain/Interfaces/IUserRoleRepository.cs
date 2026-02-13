using SasthoSoft.Domain.Entities;

namespace SasthoSoft.Domain.Interfaces;

public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(int id);
    Task<UserRole?> GetByNameAsync(string name);
    Task<IEnumerable<UserRole>> GetAllAsync();
    Task<UserRole> AddAsync(UserRole userRole);
    Task UpdateAsync(UserRole userRole);
    Task DeleteAsync(int id);
}
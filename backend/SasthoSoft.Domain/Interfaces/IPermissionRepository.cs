using SasthoSoft.Domain.Entities;

namespace SasthoSoft.Domain.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<Permission> AddAsync(Permission permission);
    Task UpdateAsync(Permission permission);
    Task DeleteAsync(int id);

    // Optional: get permissions by Role or Menu
    Task<IEnumerable<Permission>> GetByUserRoleIdAsync(int userRoleId);
    Task<IEnumerable<Permission>> GetByMenuIdAsync(int menuId);
}

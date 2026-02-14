using SasthoSoft.Domain.Entities;

namespace SasthoSoft.Domain.Interfaces;

public interface IUserRepository
{
    // --------------------------
    // Basic CRUD operations
    // --------------------------
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int userId);

    // --------------------------
    // Role-based queries
    // --------------------------
    Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
    Task<int> GetUserCountAsync();

    // --------------------------
    // Refresh token management
    // --------------------------
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task RevokeRefreshTokenAsync(int userId);
}

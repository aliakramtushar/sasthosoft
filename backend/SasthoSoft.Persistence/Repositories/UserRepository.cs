using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    // --------------------------
    // Basic CRUD using Dapper.Contrib
    // --------------------------
    public async Task<User?> GetByIdAsync(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.GetAsync<User>(userId);
    }

    public async Task<User> AddAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        var id = await connection.InsertAsync(user); // returns generated UserID
        user.UserID = (int)id;
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.UpdateAsync(user);
    }

    public async Task DeleteAsync(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var user = await connection.GetAsync<User>(userId);
        if (user != null)
        {
            await connection.DeleteAsync(user);
        }
    }

    // --------------------------
    // Complex queries using plain Dapper
    // --------------------------
    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.*
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleID = ur.UserRoleID
            WHERE u.Username = @Username";

        var result = await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                //user.Role = role;
                return user;
            },
            new { Username = username },
            splitOn: "UserRoleID"
        );

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.*
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleID = ur.UserRoleID";

        return await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                //user.Role = role;
                return user;
            },
            splitOn: "UserRoleID"
        );
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.*
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleID = ur.UserRoleID
            WHERE u.RoleID = @RoleID";

        return await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                //user.Role = role;
                return user;
            },
            new { RoleID = roleId },
            splitOn: "UserRoleID"
        );
    }

    public async Task<int> GetUserCountAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
    }

    // --------------------------
    // Refresh token management
    // --------------------------
    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT rt.*, u.*, ur.*
            FROM RefreshTokens rt
            INNER JOIN Users u ON rt.UserID = u.UserID
            INNER JOIN UserRoles ur ON u.RoleID = ur.UserRoleID
            WHERE rt.Token = @Token AND rt.IsRevoked = 0";

        var result = await connection.QueryAsync<RefreshToken, User, UserRole, RefreshToken>(
            query,
            (refreshToken, user, role) =>
            {
                //user.Role = role;
                refreshToken.User = user;
                return refreshToken;
            },
            new { Token = token },
            splitOn: "UserID,UserRoleID"
        );

        return result.FirstOrDefault();
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            INSERT INTO RefreshTokens (UserID, Token, ExpiryDate, IsRevoked)
            VALUES (@UserID, @Token, @ExpiryDate, @IsRevoked)";

        await connection.ExecuteAsync(query, refreshToken);
    }

    public async Task RevokeRefreshTokenAsync(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            UPDATE RefreshTokens 
            SET IsRevoked = 1 
            WHERE UserID = @UserID AND IsRevoked = 0";

        await connection.ExecuteAsync(query, new { UserID = userId });
    }
}

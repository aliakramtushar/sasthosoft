using Dapper;
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

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.* 
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleId = ur.Id
            WHERE u.Id = @Id";

        var result = await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Id = id },
            splitOn: "Id"
        );

        return result.FirstOrDefault();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.* 
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleId = ur.Id
            WHERE u.Username = @Username";

        var result = await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { Username = username },
            splitOn: "Id"
        );

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.* 
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleId = ur.Id";

        var users = await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            splitOn: "Id"
        );

        return users;
    }

    public async Task<User> AddAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            INSERT INTO Users (Username, PasswordHash, Email, RoleId, CreatedAt)
            OUTPUT INSERTED.Id
            VALUES (@Username, @PasswordHash, @Email, @RoleId, @CreatedAt)";

        user.Id = await connection.ExecuteScalarAsync<int>(query, user);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            UPDATE Users 
            SET Username = @Username,
                PasswordHash = @PasswordHash,
                Email = @Email,
                RoleId = @RoleId
            WHERE Id = @Id";

        await connection.ExecuteAsync(query, user);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "DELETE FROM Users WHERE Id = @Id";
        await connection.ExecuteAsync(query, new { Id = id });
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT u.*, ur.* 
            FROM Users u
            INNER JOIN UserRoles ur ON u.RoleId = ur.Id
            WHERE u.RoleId = @RoleId";

        return await connection.QueryAsync<User, UserRole, User>(
            query,
            (user, role) =>
            {
                user.Role = role;
                return user;
            },
            new { RoleId = roleId },
            splitOn: "Id"
        );
    }

    public async Task<int> GetUserCountAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT rt.*, u.*, ur.*
            FROM RefreshTokens rt
            INNER JOIN Users u ON rt.UserId = u.Id
            INNER JOIN UserRoles ur ON u.RoleId = ur.Id
            WHERE rt.Token = @Token AND rt.IsRevoked = 0";

        var result = await connection.QueryAsync<RefreshToken, User, UserRole, RefreshToken>(
            query,
            (refreshToken, user, role) =>
            {
                user.Role = role;
                refreshToken.User = user;
                return refreshToken;
            },
            new { Token = token },
            splitOn: "Id"
        );

        return result.FirstOrDefault();
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            INSERT INTO RefreshTokens (UserId, Token, ExpiryDate, IsRevoked)
            VALUES (@UserId, @Token, @ExpiryDate, @IsRevoked)";

        await connection.ExecuteAsync(query, refreshToken);
    }

    public async Task RevokeRefreshTokenAsync(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "UPDATE RefreshTokens SET IsRevoked = 1 WHERE UserId = @UserId AND IsRevoked = 0";
        await connection.ExecuteAsync(query, new { UserId = userId });
    }
}
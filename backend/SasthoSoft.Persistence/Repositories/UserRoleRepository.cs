using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Persistence.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly string _connectionString;

    public UserRoleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<UserRole?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<UserRole>(
            "SELECT * FROM UserRoles WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<UserRole?> GetByNameAsync(string name)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<UserRole>(
            "SELECT * FROM UserRoles WHERE Name = @Name",
            new { Name = name });
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<UserRole>("SELECT * FROM UserRoles");
    }

    public async Task<UserRole> AddAsync(UserRole userRole)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            INSERT INTO UserRoles (Name)
            OUTPUT INSERTED.Id
            VALUES (@Name)";

        userRole.Id = await connection.ExecuteScalarAsync<int>(query, userRole);
        return userRole;
    }

    public async Task UpdateAsync(UserRole userRole)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "UPDATE UserRoles SET Name = @Name WHERE Id = @Id";
        await connection.ExecuteAsync(query, userRole);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "DELETE FROM UserRoles WHERE Id = @Id";
        await connection.ExecuteAsync(query, new { Id = id });
    }
}
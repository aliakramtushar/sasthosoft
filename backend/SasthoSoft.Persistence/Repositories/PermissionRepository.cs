using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Persistence.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly string _connectionString;

    public PermissionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqlConnection CreateConnection()
        => new SqlConnection(_connectionString);

    public async Task<Permission?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        return await connection.GetAsync<Permission>(id);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        using var connection = CreateConnection();
        return await connection.GetAllAsync<Permission>();
    }

    public async Task<Permission> AddAsync(Permission permission)
    {
        using var connection = CreateConnection();
        var id = await connection.InsertAsync(permission);
        permission.PermissionID = (int)id;
        return permission;
    }

    public async Task UpdateAsync(Permission permission)
    {
        using var connection = CreateConnection();
        await connection.UpdateAsync(permission);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = CreateConnection();
        var entity = await connection.GetAsync<Permission>(id);
        if (entity != null)
            await connection.DeleteAsync(entity);
    }

    public async Task<IEnumerable<Permission>> GetByUserRoleIdAsync(int userRoleId)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Permission>(
            "SELECT * FROM Permissions WHERE UserRoleID = @UserRoleID",
            new { UserRoleID = userRoleId });
    }

    public async Task<IEnumerable<Permission>> GetByMenuIdAsync(int menuId)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Permission>(
            "SELECT * FROM Permissions WHERE MenuID = @MenuID",
            new { MenuID = menuId });
    }
}

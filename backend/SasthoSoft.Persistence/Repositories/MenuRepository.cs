using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Persistence.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly string _connectionString;

    public MenuRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<Menu?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.GetAsync<Menu>(id);
    }

    public async Task<IEnumerable<Menu>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.GetAllAsync<Menu>();
    }

    public async Task<int> AddAsync(Menu menu)
    {
        using var connection = new SqlConnection(_connectionString);
        var id = await connection.InsertAsync(menu);
        return id;
    }

    public async Task UpdateAsync(Menu menu)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.UpdateAsync(menu);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        var menu = await connection.GetAsync<Menu>(id);
        if (menu != null)
        {
            await connection.DeleteAsync(menu);
        }
    }
}

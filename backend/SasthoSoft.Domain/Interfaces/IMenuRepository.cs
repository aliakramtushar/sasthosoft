using SasthoSoft.Domain.Entities;

namespace SasthoSoft.Domain.Interfaces;

public interface IMenuRepository
{
    Task<Menu?> GetByIdAsync(int id);
    Task<IEnumerable<Menu>> GetAllAsync();
    Task<int> AddAsync(Menu menu);
    Task UpdateAsync(Menu menu);
    Task DeleteAsync(int id);
}

using SasthoSoft.Application.DTOs;

namespace SasthoSoft.Application.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAllMenusAsync();
    Task<MenuDto?> GetMenuByIdAsync(int id);
    Task<MenuDto> CreateMenuAsync(MenuDto.Create createDto);
    Task UpdateMenuAsync(int id, MenuDto.Update updateDto);
    Task DeleteMenuAsync(int id);

    // Optional: build tree structure
    Task<IEnumerable<MenuDto>> GetMenuTreeAsync();
}

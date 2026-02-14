using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Application.Services;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<IEnumerable<MenuDto>> GetAllMenusAsync()
    {
        var menus = await _menuRepository.GetAllAsync();
        return menus.Select(MapToDto);
    }

    public async Task<MenuDto?> GetMenuByIdAsync(int id)
    {
        var menu = await _menuRepository.GetByIdAsync(id);
        return menu == null ? null : MapToDto(menu);
    }

    public async Task<MenuDto> CreateMenuAsync(MenuDto.Create createDto)
    {
        var menu = new Menu
        {
            MenuName = createDto.MenuName,
            ParentID = createDto.ParentID,
            Url = createDto.Url,
            Icon = createDto.Icon,
            DisplayOrder = createDto.DisplayOrder,
            HoverText = createDto.HoverText,
            IsActive = true,
            IsDeleted = false
        };

        var id = await _menuRepository.AddAsync(menu);
        menu.MenuID = id;

        return MapToDto(menu);
    }

    public async Task UpdateMenuAsync(int id, MenuDto.Update updateDto)
    {
        var menu = await _menuRepository.GetByIdAsync(id);
        if (menu == null) throw new KeyNotFoundException("Menu not found");

        menu.MenuName = updateDto.MenuName;
        menu.ParentID = updateDto.ParentID;
        menu.Url = updateDto.Url;
        menu.Icon = updateDto.Icon;
        menu.DisplayOrder = updateDto.DisplayOrder;
        menu.HoverText = updateDto.HoverText;
        menu.IsActive = updateDto.IsActive;
        menu.IsDeleted = updateDto.IsDeleted;

        await _menuRepository.UpdateAsync(menu);
    }

    public async Task DeleteMenuAsync(int id)
    {
        await _menuRepository.DeleteAsync(id);
    }

    // Build tree structure
    public async Task<IEnumerable<MenuDto>> GetMenuTreeAsync()
    {
        var menus = (await _menuRepository.GetAllAsync()).ToList();
        var lookup = menus.ToDictionary(m => m.MenuID);

        foreach (var menu in menus)
        {
            if (menu.ParentID.HasValue && lookup.ContainsKey(menu.ParentID.Value))
            {
                lookup[menu.ParentID.Value].Children.Add(menu);
            }
        }

        var rootMenus = menus.Where(m => !m.ParentID.HasValue || m.ParentID == 0).ToList();
        return rootMenus.Select(MapToDtoWithChildren);
    }

    // -----------------------------
    // Private mappers
    // -----------------------------
    private MenuDto MapToDto(Menu menu)
    {
        return new MenuDto
        {
            MenuID = menu.MenuID,
            MenuName = menu.MenuName,
            ParentID = menu.ParentID,
            Url = menu.Url,
            Icon = menu.Icon,
            DisplayOrder = menu.DisplayOrder,
            HoverText = menu.HoverText,
            IsActive = menu.IsActive,
            IsDeleted = menu.IsDeleted
        };
    }

    private MenuDto MapToDtoWithChildren(Menu menu)
    {
        var dto = MapToDto(menu);
        dto.Children = menu.Children.Select(MapToDtoWithChildren).ToList();
        return dto;
    }
}

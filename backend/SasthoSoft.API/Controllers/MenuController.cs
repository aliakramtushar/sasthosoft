using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;

namespace SasthoSoft.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    // --------------------------
    // GET: api/Menu
    // Get all menus
    // --------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MenuDto>>> GetAll()
    {
        var menus = await _menuService.GetAllMenusAsync();
        return Ok(menus);
    }

    // --------------------------
    // GET: api/Menu/{id}
    // Get menu by ID
    // --------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<MenuDto>> GetById(int id)
    {
        var menu = await _menuService.GetMenuByIdAsync(id);
        if (menu == null)
            return NotFound();

        return Ok(menu);
    }

    // --------------------------
    // POST: api/Menu
    // Create a new menu
    // --------------------------
    [HttpPost]
    public async Task<ActionResult<MenuDto>> Create([FromBody] MenuDto.Create createMenuDto)
    {
        var menu = await _menuService.CreateMenuAsync(createMenuDto);
        return CreatedAtAction(nameof(GetById), new { id = menu.MenuID }, menu);
    }

    // --------------------------
    // PUT: api/Menu/{id}
    // Update an existing menu
    // --------------------------
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MenuDto.Update updateMenuDto)
    {
        await _menuService.UpdateMenuAsync(id, updateMenuDto);
        return NoContent();
    }

    // --------------------------
    // DELETE: api/Menu/{id}
    // Delete a menu
    // --------------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _menuService.DeleteMenuAsync(id);
        return NoContent();
    }

    // --------------------------
    // GET: api/Menu/tree
    // Get menus in a tree structure
    // --------------------------
    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<MenuDto>>> GetTree()
    {
        var tree = await _menuService.GetMenuTreeAsync();
        return Ok(tree);
    }
}

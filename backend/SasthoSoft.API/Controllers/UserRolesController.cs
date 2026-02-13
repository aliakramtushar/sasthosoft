using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;

namespace SasthoSoft.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRolesController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetAll()
    {
        var roles = await _userRoleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserRoleDto>> GetById(int id)
    {
        var role = await _userRoleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<UserRoleDto>> Create(CreateUserRoleDto createRoleDto)
    {
        var role = await _userRoleService.CreateRoleAsync(createRoleDto);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserRoleDto updateRoleDto)
    {
        await _userRoleService.UpdateRoleAsync(id, updateRoleDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userRoleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;
using SasthoSoft.Domain.Enums;

namespace SasthoSoft.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    // ------------------------------
    // GET: api/permissions
    // ------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
    {
        var permissions = await _permissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    // ------------------------------
    // GET: api/permissions/{id}
    // ------------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionDto>> GetById(int id)
    {
        var permission = await _permissionService.GetPermissionByIdAsync(id);
        if (permission == null)
            return NotFound();

        return Ok(permission);
    }

    // ------------------------------
    // POST: api/permissions
    // ------------------------------
    [HttpPost]
    public async Task<ActionResult<PermissionDto>> Create(PermissionDto.Create createDto)
    {
        var permission = await _permissionService.CreatePermissionAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = permission.PermissionID }, permission);
    }

    // ------------------------------
    // PUT: api/permissions/{id}
    // ------------------------------
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, PermissionDto.Update updateDto)
    {
        await _permissionService.UpdatePermissionAsync(id, updateDto);
        return NoContent();
    }

    // ------------------------------
    // DELETE: api/permissions/{id}
    // ------------------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _permissionService.DeletePermissionAsync(id);
        return NoContent();
    }

    // ------------------------------
    // GET: api/permissions/byrole/{userRoleId}
    // ------------------------------
    [HttpGet("byrole/{userRoleId}")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetByRole(int userRoleId)
    {
        var permissions = await _permissionService.GetPermissionsByRoleAsync(userRoleId);
        return Ok(permissions);
    }

    // ------------------------------
    // GET: api/permissions/bymenu/{menuId}
    // ------------------------------
    [HttpGet("bymenu/{menuId}")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetByMenu(int menuId)
    {
        var permissions = await _permissionService.GetPermissionsByMenuAsync(menuId);
        return Ok(permissions);
    }

    // ------------------------------
    // GET: api/permissions/check
    // Example: /api/permissions/check?roleId=1&menuId=5&permissionType=Edit
    // ------------------------------
    [HttpGet("check")]
    public async Task<ActionResult<bool>> HasPermission(
        [FromQuery] int roleId,
        [FromQuery] int menuId,
        [FromQuery] AppEnum.PermissionType permissionType)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(roleId, menuId, permissionType);
        return Ok(hasPermission);
    }
}

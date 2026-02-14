using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Enums;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    // Get all permissions
    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissions = await _permissionRepository.GetAllAsync();
        return permissions.Select(MapToPermissionDto);
    }

    // Get permission by ID
    public async Task<PermissionDto?> GetPermissionByIdAsync(int id)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        return permission == null ? null : MapToPermissionDto(permission);
    }

    // Create new permission
    public async Task<PermissionDto> CreatePermissionAsync(PermissionDto.Create createDto)
    {
        // Optional: check for duplicates
        var existingPermissions = await _permissionRepository.GetByUserRoleIdAsync(createDto.UserRoleID);
        if (existingPermissions.Any(p =>
            p.MenuID == createDto.MenuID &&
            p.PermissionType == createDto.PermissionType))
        {
            throw new InvalidOperationException("Permission already exists for this role and menu.");
        }

        var permission = new Permission
        {
            PermissionType = createDto.PermissionType,
            MenuID = createDto.MenuID,
            UserRoleID = createDto.UserRoleID,
            IsActive = true,
            IsDelete = false,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createDto.CreatedBy
        };

        var createdPermission = await _permissionRepository.AddAsync(permission);
        return MapToPermissionDto(createdPermission);
    }

    // Update existing permission
    public async Task UpdatePermissionAsync(int id, PermissionDto.Update updateDto)
    {
        var permission = await _permissionRepository.GetByIdAsync(id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found.");

        permission.PermissionType = updateDto.PermissionType;
        permission.IsActive = updateDto.IsActive ? true : false;
        permission.IsDelete = updateDto.IsDelete ? true : false;

        await _permissionRepository.UpdateAsync(permission);
    }

    // Delete permission
    public async Task DeletePermissionAsync(int id)
    {
        await _permissionRepository.DeleteAsync(id);
    }

    // Get permissions by role
    public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(int userRoleId)
    {
        var permissions = await _permissionRepository.GetByUserRoleIdAsync(userRoleId);
        return permissions.Select(MapToPermissionDto);
    }

    // Get permissions by menu
    public async Task<IEnumerable<PermissionDto>> GetPermissionsByMenuAsync(int menuId)
    {
        var permissions = await _permissionRepository.GetByMenuIdAsync(menuId);
        return permissions.Select(MapToPermissionDto);
    }

    // Check if a role has a specific permission for a menu
    public async Task<bool> HasPermissionAsync(int userRoleId, int menuId, AppEnum.PermissionType permissionType)
    {
        var permissions = await _permissionRepository.GetByUserRoleIdAsync(userRoleId);
        return permissions.Any(p =>
            p.MenuID == menuId &&
            p.PermissionType == permissionType &&
            p.IsActive == true &&
            p.IsDelete == false);
    }

    // ------------------------------
    // Private mapper
    // ------------------------------
    private PermissionDto MapToPermissionDto(Permission permission)
    {
        return new PermissionDto
        {
            PermissionID = permission.PermissionID,
            PermissionType = (AppEnum.PermissionType)permission.PermissionType,
            MenuID = permission.MenuID,
            UserRoleID = permission.UserRoleID,
            IsActive = permission.IsActive == true,
            IsDelete = permission.IsDelete == false,
            CreatedDate = permission.CreatedDate,
            CreatedBy = permission.CreatedBy
        };
    }
}

using SasthoSoft.Application.DTOs;
using SasthoSoft.Domain.Enums;

namespace SasthoSoft.Application.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
    Task<PermissionDto?> GetPermissionByIdAsync(int id);
    Task<PermissionDto> CreatePermissionAsync(PermissionDto.Create createPermissionDto);
    Task UpdatePermissionAsync(int id, PermissionDto.Update updatePermissionDto);
    Task DeletePermissionAsync(int id);

    // Optional helper methods
    Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(int userRoleId);
    Task<IEnumerable<PermissionDto>> GetPermissionsByMenuAsync(int menuId);
    Task<bool> HasPermissionAsync(int userRoleId, int menuId, AppEnum.PermissionType permissionType);
}

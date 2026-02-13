using SasthoSoft.Application.DTOs;

namespace SasthoSoft.Application.Interfaces;

public interface IUserRoleService
{
    Task<IEnumerable<UserRoleDto>> GetAllRolesAsync();
    Task<UserRoleDto?> GetRoleByIdAsync(int id);
    Task<UserRoleDto> CreateRoleAsync(CreateUserRoleDto createRoleDto);
    Task UpdateRoleAsync(int id, UpdateUserRoleDto updateRoleDto);
    Task DeleteRoleAsync(int id);
}
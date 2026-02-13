using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllRolesAsync()
    {
        var roles = await _userRoleRepository.GetAllAsync();
        return roles.Select(MapToUserRoleDto);
    }

    public async Task<UserRoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _userRoleRepository.GetByIdAsync(id);
        return role == null ? null : MapToUserRoleDto(role);
    }

    public async Task<UserRoleDto> CreateRoleAsync(CreateUserRoleDto createRoleDto)
    {
        var existingRole = await _userRoleRepository.GetByNameAsync(createRoleDto.Name);
        if (existingRole != null)
        {
            throw new InvalidOperationException("Role already exists");
        }

        var role = new UserRole
        {
            Name = createRoleDto.Name
        };

        var createdRole = await _userRoleRepository.AddAsync(role);
        return MapToUserRoleDto(createdRole);
    }

    public async Task UpdateRoleAsync(int id, UpdateUserRoleDto updateRoleDto)
    {
        var role = await _userRoleRepository.GetByIdAsync(id);
        if (role == null)
        {
            throw new KeyNotFoundException("Role not found");
        }

        role.Name = updateRoleDto.Name;
        await _userRoleRepository.UpdateAsync(role);
    }

    public async Task DeleteRoleAsync(int id)
    {
        await _userRoleRepository.DeleteAsync(id);
    }

    private UserRoleDto MapToUserRoleDto(UserRole role)
    {
        return new UserRoleDto
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}
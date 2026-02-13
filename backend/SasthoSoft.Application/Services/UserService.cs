using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;

namespace SasthoSoft.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserService(IUserRepository userRepository, IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToUserDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(createUserDto.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        var role = await _userRoleRepository.GetByIdAsync(createUserDto.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException("Role not found");
        }

        var user = new User
        {
            Username = createUserDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            Email = createUserDto.Email,
            RoleId = createUserDto.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToUserDto(createdUser);
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            user.Email = updateUserDto.Email;
        }

        if (updateUserDto.RoleId.HasValue)
        {
            var role = await _userRoleRepository.GetByIdAsync(updateUserDto.RoleId.Value);
            if (role == null)
            {
                throw new InvalidOperationException("Role not found");
            }
            user.RoleId = updateUserDto.RoleId.Value;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
        }

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name ?? string.Empty,
            CreatedAt = user.CreatedAt
        };
    }
}
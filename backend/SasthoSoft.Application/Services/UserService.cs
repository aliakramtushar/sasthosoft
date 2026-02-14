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
            throw new InvalidOperationException("Username already exists");

        var role = await _userRoleRepository.GetByIdAsync(createUserDto.RoleID);
        if (role == null)
            throw new InvalidOperationException("Role not found");

        var user = new User
        {
            Username = createUserDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            Email = createUserDto.Email,
            RoleID = createUserDto.RoleID,
            CreatedDate = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToUserDto(createdUser);
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        if (!string.IsNullOrEmpty(updateUserDto.Email))
            user.Email = updateUserDto.Email;

        if (updateUserDto.RoleID.HasValue)
        {
            var role = await _userRoleRepository.GetByIdAsync(updateUserDto.RoleID.Value);
            if (role == null)
                throw new InvalidOperationException("Role not found");

            user.RoleID = updateUserDto.RoleID.Value;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
    }

    // --------------------------
    // Mapper: User -> UserDto
    // --------------------------
    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            UserID = user.UserID,
            Username = user.Username,
            Email = user.Email,
            RoleID = user.RoleID,
            //RoleName = user.Role?.UserRoleName ?? string.Empty,
            CreatedDate = user.CreatedDate
        };
    }
}

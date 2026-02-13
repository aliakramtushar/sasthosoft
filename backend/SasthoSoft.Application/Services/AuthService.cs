using SasthoSoft.Application.DTOs;
using SasthoSoft.Application.Interfaces;
using SasthoSoft.Domain.Entities;
using SasthoSoft.Domain.Interfaces;
using SasthoSoft.Infrastructure.Interfaces;

namespace SasthoSoft.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _userRepository.RevokeRefreshTokenAsync(user.Id);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _userRepository.AddRefreshTokenAsync(refreshTokenEntity);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = MapToUserDto(user)
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var token = await _userRepository.GetRefreshTokenAsync(refreshToken);
        if (token == null || token.ExpiryDate <= DateTime.UtcNow || token.IsRevoked)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = await _userRepository.GetByIdAsync(token.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        token.IsRevoked = true;
        await _userRepository.UpdateAsync(user);

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _userRepository.AddRefreshTokenAsync(newRefreshTokenEntity);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = MapToUserDto(user)
        };
    }

    public async Task LogoutAsync(int userId)
    {
        await _userRepository.RevokeRefreshTokenAsync(userId);
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
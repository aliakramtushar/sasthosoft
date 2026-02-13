using SasthoSoft.Domain.Entities;

namespace SasthoSoft.Infrastructure.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
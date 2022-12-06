using RestAuth.Domain.Entities;
using RestAuth.Domain.Helpers;
using RestAuth.Domain.Models;

namespace RestAuth.Domain.Interfaces.Services
{
    public interface IJWTService
    {
        Task<AuthenticationResponseModel> CreateAuthentication(Guid userId, string email, IEnumerable<Role> roles = null, RefreshTokenData refreshTokenData = null);

        Task<RefreshTokenData> ValidateRefreshToken(string refreshToken, Guid userId);

        bool ValidateAccessToken(string token, Guid userId);
    }
}
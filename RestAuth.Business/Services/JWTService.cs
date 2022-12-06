using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Helpers;
using RestAuth.Domain.Interfaces.Services;
using RestAuth.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace RestAuth.Business.Services
{
    public class JWTService : IJWTService
    {
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly IDistributedCache _cache;

        public JWTService(SigningConfigurations signingConfigurations, TokenConfigurations tokenConfigurations, IDistributedCache cache)
        {
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _cache = cache;
        }

        public async Task<AuthenticationResponseModel> CreateAuthentication(Guid userId, string email, IEnumerable<Role> roles = null, RefreshTokenData refreshTokenData = null)
        {
            try
            {
                ClaimsIdentity identity = new(
                        new GenericIdentity(email, "Login"),
                        new[] {
                        new Claim("User.Identity.Name", userId.ToString())
                        }
                    );

                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                    }
                }

                var creationDate = DateTime.UtcNow;
                var expirationDate = creationDate +
                    TimeSpan.FromSeconds(_tokenConfigurations.AccessTokenValidity);

                var finalExpirationDate = creationDate +
                    TimeSpan.FromSeconds(_tokenConfigurations.RefreshTokenValidity);

                string refreshToken = Guid.NewGuid().ToString("N");

                if (refreshTokenData != null)
                {
                    finalExpirationDate = refreshTokenData.FinalExpiration;
                    refreshToken = refreshTokenData.RefreshToken;
                }

                string accessToken = GenerateAccessToken(identity, creationDate, expirationDate);

                await SendAccessTokenToRedis(accessToken, userId, expirationDate);
                await SendRefreshTokenToRedis(refreshToken, userId, finalExpirationDate);

                return new AuthenticationResponseModel
                {
                    AccessToken = accessToken,
                    AccessTokenExpiration = expirationDate,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = finalExpirationDate
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Falha ao criar autenticação. Erro: {ex.Message}");
            }
        }

        public async Task<RefreshTokenData> ValidateRefreshToken(string refreshToken, Guid userId)
        {
            return await GetRefreshTokenFromRedis(refreshToken, userId);
        }

        public bool ValidateAccessToken(string token, Guid userId)
        {
            var storedToken = GetAccessTokenFromRedis(userId);
            if (storedToken == null)
                return false;

            return token.Equals(storedToken);
        }

        private async Task<RefreshTokenData> GetRefreshTokenFromRedis(string refreshToken, Guid userId)
        {
            var storedToken = await _cache.GetStringAsync(refreshToken);
            if (String.IsNullOrEmpty(storedToken))
            {
                return null;
            }

            var refreshTokenData = JsonSerializer.Deserialize<RefreshTokenData>(storedToken);

            if (refreshTokenData.UserId.Equals(userId.ToString()))
            {
                return refreshTokenData;
            }

            return null;
        }

        private string GetAccessTokenFromRedis(Guid userId)
        {
            var storedToken = _cache.GetString(userId.ToString());
            if (String.IsNullOrEmpty(storedToken))
            {
                return null;
            }

            return storedToken;
        }

        private async Task SendAccessTokenToRedis(string accessToken, Guid userId, DateTime finalExpiration)
        {
            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(finalExpiration);

            await _cache.SetStringAsync(userId.ToString(), accessToken, cacheOptions);
        }

        private async Task SendRefreshTokenToRedis(string refreshToken, Guid userId, DateTime finalExpiration)
        {
            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(finalExpiration);
            var refreshTokenData = new RefreshTokenData
            {
                RefreshToken = refreshToken,
                UserId = userId.ToString(),
                FinalExpiration = finalExpiration
            };

            await _cache.SetStringAsync(refreshToken, JsonSerializer.Serialize(refreshTokenData), cacheOptions);
        }

        private string GenerateAccessToken(ClaimsIdentity identity, DateTime creationDate, DateTime expirationDate)
        {
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = creationDate,
                Expires = expirationDate
            });

            return handler.WriteToken(securityToken);
        }
    }
}
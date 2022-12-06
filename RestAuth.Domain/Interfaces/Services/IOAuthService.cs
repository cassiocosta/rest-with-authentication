using RestAuth.Domain.Entities;
using RestAuth.Domain.Models;

namespace RestAuth.Domain.Interfaces.Services
{
    public interface IOAuthService
    {
        Task<AuthenticationResponseModel> Register(string name, string email, string password, Guid roleId);

        Task<AuthenticationResponseModel> Authenticate(string email, string password);

        Task<AuthenticationResponseModel> RefreshToken(string email, string token);

        Task<User> ChangePassword(string email, string oldPassword, string newPassword);
    }
}
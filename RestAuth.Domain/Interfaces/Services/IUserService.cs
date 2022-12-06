using RestAuth.Domain.Entities;
using RestAuth.Domain.Models;

namespace RestAuth.Domain.Interfaces.Services
{
    public interface IUserService : IServiceBase<User>
    {
        Task<AuthenticationResponseModel> Register(RegistrationModel registration);

        Task<AuthenticationResponseModel> Authenticate(string email, string password);

        Task<AuthenticationResponseModel> RefreshToken(string email, string token);

        Task<bool> ChangePassword(ChangePasswordModel changePassword);
    }
}
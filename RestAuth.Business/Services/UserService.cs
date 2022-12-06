using RestAuth.Business.Services;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Helpers;
using RestAuth.Domain.Interfaces.Repositories;
using RestAuth.Domain.Interfaces.Services;
using RestAuth.Domain.Models;

namespace RestAuth.Business.Services
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IOAuthService _oAuthService;

        public UserService(IUserRepository userRepository,
            IOAuthService oAuthService)
            : base(userRepository)
        {
            _oAuthService = oAuthService;
        }

        public async Task<AuthenticationResponseModel> Register(RegistrationModel registration)
        {
            if (registration == null)
                return null;

            var registrationResultModel = await _oAuthService.Register(registration.Name.Trim(), registration.Email.ToLower().Trim(), registration.Password.Trim(), Guid.Parse(RoleHelper.USER_ROLE_ID));

            return registrationResultModel;
        }

        public async Task<AuthenticationResponseModel> Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var authenticationResponseModel = await _oAuthService.Authenticate(email, password);
            if (authenticationResponseModel == null)
                throw new ApplicationException("Ocorreu um erro ao executar a autenticação");

            return authenticationResponseModel;
        }

        public async Task<AuthenticationResponseModel> RefreshToken(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var authenticationResponseModel = await _oAuthService.RefreshToken(email, token);

            return authenticationResponseModel;
        }

        public async Task<bool> ChangePassword(ChangePasswordModel changePassword)
        {
            if (changePassword== null)
                throw new ArgumentNullException("Parâmetro changePasswordModel");

            await _oAuthService.ChangePassword(changePassword.Email, changePassword.OldPassword, changePassword.NewPassword);

            return true;
        }
    }
}
using RestAuth.Domain.Entities;
using RestAuth.Domain.Helpers;
using RestAuth.Domain.Interfaces.Repositories;
using RestAuth.Domain.Interfaces.Services;
using RestAuth.Domain.Models;
using System.Text;

namespace RestAuth.Business.Services
{
    public class OAuthService : IOAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IJWTService _jwtService;

        public OAuthService(IUserRepository userRepository,
            IRoleService roleService,
            IJWTService jWTService)
        {
            _userRepository = userRepository;
            _roleService = roleService;
            _jwtService = jWTService;
        }

        public async Task<AuthenticationResponseModel> Register(string name, string email, string password, Guid roleId)
        {
            if (!await IsEmailAvailable(email))
                throw new ApplicationException("E-mail já cadastrado na base de dados");

            var validUser = await RegisterUser(name, email, password, roleId);

            var authenticationResultModel = await CreateAuthentication(validUser.Id, validUser.Email, validUser.UserHasRoles.Select(x => x.Role).ToList());

            return authenticationResultModel;
        }

        public async Task<AuthenticationResponseModel> Authenticate(string email, string password)
        {
            var validUser = await _userRepository.GetByEmail(email);
            if (validUser == null)
                throw new ApplicationException("Usuário inválido");

            if (!RSACipherHelper.ValidateEncryptedData(password, Encoding.UTF8.GetString(validUser.Password)))
                throw new ApplicationException("Usuário inválido");

            var authenticationResponseModel = await CreateAuthentication(validUser.Id, validUser.Email, validUser.UserHasRoles.Select(x => x.Role).ToList());

            return authenticationResponseModel;
        }

        public async Task<AuthenticationResponseModel> RefreshToken(string email, string token)
        {
            var validUser = await _userRepository.GetByEmail(email);
            if (validUser == null)
                throw new ApplicationException("Usuário não encontrado");

            var refreshTokenData = await _jwtService.ValidateRefreshToken(token, validUser.Id);
            if (refreshTokenData == null)
                throw new ApplicationException("Refresh token inválido");

            var authenticationResponseModel = await CreateAuthentication(validUser.Id, validUser.Email, validUser.UserHasRoles.Select(x => x.Role).ToList());

            return authenticationResponseModel;
        }

        public async Task<User> ChangePassword(string email, string oldPassword, string newPassword)
        {
            var validUser = await _userRepository.GetByEmail(email);
            if (validUser == null)
                throw new ApplicationException("Usuário inválido");

            if (!RSACipherHelper.ValidateEncryptedData(oldPassword, Encoding.UTF8.GetString(validUser.Password)))
                throw new ApplicationException("Usuário inválido");

            validUser.Password = Encoding.UTF8.GetBytes(RSACipherHelper.EncryptString(newPassword));
            await _userRepository.AddOrUpdateAsync(validUser);

            return validUser;
        }

        private async Task<bool> IsEmailAvailable(string email)
        {
            return await _userRepository.IsEmailAvailable(email);
        }

        private async Task<User> RegisterUser(string name, string email, string password, Guid roleId)
        {
            var role = await _roleService.GetByIdAsync(roleId);

            var validUser = new User
            {
                Name = name,
                Email = email,
                Password = Encoding.UTF8.GetBytes(RSACipherHelper.EncryptString(password)),
                UserHasRoles = new List<UserHasRoles> { new UserHasRoles { RoleId = role.Id, Role = role } }
            };

            await _userRepository.AddOrUpdateAsync(validUser);
            return validUser;
        }

        private async Task<AuthenticationResponseModel> CreateAuthentication(Guid userId, string email, IEnumerable<Role> roles)
        {
            return await _jwtService.CreateAuthentication(userId, email, roles, null);
        }
    }
}
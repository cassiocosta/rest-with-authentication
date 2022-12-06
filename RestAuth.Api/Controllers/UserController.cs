using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Services;
using RestAuth.Domain.Models;

namespace RestAuth.Api.Controllers
{
    public class UserController : EntityController<User>
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
            : base(userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationModel authentication)
        {
            try
            {
                var validation = await _userService.Authenticate(authentication.Email, authentication.Password);

                if (validation == null)
                    return Unauthorized();

                return Ok(validation);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registration)
        {
            try
            {
                var user = await _userService.Register(registration);

                return Ok(user);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return DefaultBadRequest(ex.Message);
            }
        }
    }
}
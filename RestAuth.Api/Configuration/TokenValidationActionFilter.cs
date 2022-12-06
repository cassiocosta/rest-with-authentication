using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using RestAuth.Domain.Helpers;
using RestAuth.Domain.Interfaces.Services;

namespace RestAuth.Api.Configuration
{
    public class TokenValidationActionFilter : IAuthorizationFilter
    {
        private IJWTService _jwtService;

        public bool IsTestEnvironment { get; }

        public TokenValidationActionFilter(IWebHostEnvironment environment)
        {
            IsTestEnvironment = environment.EnvironmentName.Equals("Test");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _jwtService = context.HttpContext.RequestServices.GetService<IJWTService>();

            // Refresh_token
            if (context.HttpContext.Request.Headers.Any(x => x.Value.Equals("refresh_token")) && context.HttpContext.Request.Path.Value.Contains("refresh-token"))
                return;

            // Rotas que tenham o annotation AllowAnonymous
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return;

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (String.IsNullOrEmpty(token))
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            var userId = JWTHelper.GetUserIdFromToken(token);

            token = token.Replace("Bearer ", String.Empty);
            if (!_jwtService.ValidateAccessToken(token, userId))
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace RestAuth.Api.Configuration
{
    public class TokenValidationAttribute : TypeFilterAttribute
    {
        public TokenValidationAttribute()
            : base(typeof(TokenValidationActionFilter))
        {
        }
    }
}
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace RestAuth.Domain.Helpers
{
    public static class JWTHelper
    {
        public static Guid GetUserIdFromToken(StringValues token)
        {
            Guid userId = default;

            if (String.IsNullOrEmpty(token))
                throw new ApplicationException("AccessToken não informado");

            var stream = token.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var tokenObject = handler.ReadToken(stream) as JwtSecurityToken;

            if (tokenObject.Payload.ContainsKey("User.Identity.Name"))
            {
                tokenObject.Payload.TryGetValue("User.Identity.Name", out object userIdObject);
                userId = Guid.Parse(userIdObject.ToString());
            }

            return userId;
        }
    }
}
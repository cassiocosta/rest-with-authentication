using System.Text.Json.Serialization;

namespace RestAuth.Domain.Models
{
    public class AuthenticationResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
}
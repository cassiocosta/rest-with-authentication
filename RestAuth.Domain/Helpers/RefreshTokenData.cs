namespace RestAuth.Domain.Helpers
{
    public class RefreshTokenData
    {
        public string RefreshToken { get; set; }

        public string UserId { get; set; }

        public DateTime FinalExpiration { get; set; }
    }
}
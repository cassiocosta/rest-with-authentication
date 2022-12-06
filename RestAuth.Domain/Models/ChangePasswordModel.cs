namespace RestAuth.Domain.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string Email { get; set; }
    }
}
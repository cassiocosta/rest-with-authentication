namespace RestAuth.Domain.Entities
{
    public class UserHasRoles
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Role Role { get; set; }
        public Guid RoleId { get; set; }
    }
}
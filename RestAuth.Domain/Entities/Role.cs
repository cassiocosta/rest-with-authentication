namespace RestAuth.Domain.Entities
{
    public class Role : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<UserHasRoles> UserHasRoles { get; set; }
    }
}
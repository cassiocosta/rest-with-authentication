using Newtonsoft.Json;

namespace RestAuth.Domain.Entities
{
    public class User : Entity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public byte[] Password { get; set; }

        public ICollection<UserHasRoles> UserHasRoles { get; set; }
    }
}
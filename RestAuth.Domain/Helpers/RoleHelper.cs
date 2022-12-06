using RestAuth.Domain.Entities.Enum;

namespace RestAuth.Domain.Helpers
{
    public static class RoleHelper
    {
        public const string USER_ROLE_ID = "d69a30e0-fb96-416e-bd1c-7c978a8b6584";
        public const string ADMIN_USER_ROLE_ID = "8bb4f0bb-2453-47fa-9a90-30aead035553";

        public static readonly Dictionary<RoleType, string> RoleIds = new()
        {
            { RoleType.User, USER_ROLE_ID },
            { RoleType.AdminUser, ADMIN_USER_ROLE_ID },
        };
    }
}
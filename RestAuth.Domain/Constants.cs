namespace RestAuth.Domain
{
    public static class Constants
    {
        #region Roles

        public static readonly Dictionary<int, string> RoleIds = new()
        {
            { 1, "d69a30e0-fb96-416e-bd1c-7c978a8b6584" },
            { 2, "8bb4f0bb-2453-47fa-9a90-30aead035553" }
        };

        public static readonly Dictionary<int, string> RoleNames = new()
        {
            { 1, "FinalUser" },
            { 2, "AdminUser" }
        };

        public static readonly Dictionary<int, string> RoleDescriptions = new()
        {
            { 1, "Destinado ao usuário padrão do sistema" },
            { 2, "Destinado ao usuário Admin" }
        };

        #endregion Roles
    }
}
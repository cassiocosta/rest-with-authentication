using RestAuth.Data.Context;
using RestAuth.Data.Repositories;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Repositories;

namespace RestAuth.Infra.Data.Repositories
{
    public class RoleRepository : RepositoryBase<RestAuthContext, Role>, IRoleRepository
    {
        public RoleRepository(RestAuthContext context)
            : base(context)
        {
        }
    }
}
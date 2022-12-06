using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Repositories;
using RestAuth.Domain.Interfaces.Services;

namespace RestAuth.Business.Services
{
    public class RoleService : ServiceBase<Role>, IRoleService
    {
        public RoleService(IRoleRepository roleRepository)
            : base(roleRepository)
        {
        }
    }
}
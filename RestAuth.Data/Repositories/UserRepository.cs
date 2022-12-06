using Microsoft.EntityFrameworkCore;
using RestAuth.Data.Context;
using RestAuth.Data.Repositories;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Repositories;

namespace RestAuth.Infra.Data.Repositories
{
    public class UserRepository : RepositoryBase<RestAuthContext, User>, IUserRepository
    {
        public UserRepository(RestAuthContext context)
            : base(context)
        {
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.User
                .Include(x => x.UserHasRoles)
                    .ThenInclude(x => x.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email.Equals(email));
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(x => x.Email.Equals(email));

            return user == null;
        }
    }
}
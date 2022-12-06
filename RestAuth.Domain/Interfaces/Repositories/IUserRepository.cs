using RestAuth.Domain.Entities;

namespace RestAuth.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<bool> IsEmailAvailable(string email);

        Task<User> GetByEmail(string email);
    }
}
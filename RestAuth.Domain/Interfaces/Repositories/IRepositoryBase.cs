using RestAuth.Domain.Entities;
using System.Linq.Expressions;

namespace RestAuth.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity>
        where TEntity : IEntity<Guid>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsNoTrackingAsync();

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, bool excludeDeleted = false);

        Task<IEnumerable<TEntity>> GetPaginated(int page, int pageSize);

        Task<TEntity> GetByIdAsync(Guid id);

        Task<TEntity> GetByIdAsNoTrackingAsync(Guid id);

        Task<TEntity> AddOrUpdateAsync(TEntity entity);

        Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities);

        void Delete(Guid id);

        void Delete(IEnumerable<Guid> ids);
    }
}
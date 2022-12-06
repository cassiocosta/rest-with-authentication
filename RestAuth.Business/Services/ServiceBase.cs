using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Repositories;
using RestAuth.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RestAuth.Business.Services
{
    public abstract class ServiceBase<TEntity> : IServiceBase<TEntity>
        where TEntity : IEntity<Guid>
    {
        private readonly IRepositoryBase<TEntity> _repositoryBase;

        public ServiceBase(IRepositoryBase<TEntity> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _repositoryBase.GetAllAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsNoTrackingAsync()
        {
            return await _repositoryBase.GetAllAsNoTrackingAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, bool excludeDeleted = false)
        {
            return await _repositoryBase.GetAllAsync(predicate, excludeDeleted);
        }

        public Task<IEnumerable<TEntity>> GetPaginated(int page, int pageSize)
        {
            return _repositoryBase.GetPaginated(page, pageSize);
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _repositoryBase.GetByIdAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsNoTrackingAsync(Guid id)
        {
            return await _repositoryBase.GetByIdAsNoTrackingAsync(id);
        }

        public virtual async Task<TEntity> AddOrUpdateAsync(TEntity obj)
        {
            return await _repositoryBase.AddOrUpdateAsync(obj);
        }

        public async Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities)
        {
            return await _repositoryBase.AddOrUpdateAsync(entities);
        }

        public virtual void Delete(Guid id)
        {
            _repositoryBase.Delete(id);
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            _repositoryBase.Delete(ids);
        }
    }
}
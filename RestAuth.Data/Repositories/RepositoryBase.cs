using Microsoft.EntityFrameworkCore;
using RestAuth.Data.Context;
using RestAuth.Domain.Entities;
using RestAuth.Domain.Interfaces.Behavior;
using RestAuth.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RestAuth.Data.Repositories
{
    public abstract class RepositoryBase<TContext, TEntity> : IRepositoryBase<TEntity>, IDisposable
        where TEntity : class, IEntity<Guid>
        where TContext : RestAuthContext
    {
        protected TContext _context;

        public RepositoryBase(TContext context)
        {
            _context = context;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await InternalAdd(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<TEntity> AddOrUpdateAsync(TEntity entity)
        {
            if (entity.Id.Equals(default))
            {
                await AddAsync(entity);
            }
            else
            {
                await UpdateAsync(entity);
            }

            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities)
        {
            List<TEntity> persistedEntities = new();

            foreach (var item in entities.ToList())
            {
                persistedEntities.Add(await AddOrUpdateAsync(item));
            }

            return persistedEntities;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsNoTrackingAsync()
        {
            var query = (IQueryable<TEntity>)_context.Set<TEntity>();

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = (IQueryable<TEntity>)_context.Set<TEntity>();

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, bool excludeDeleted = false)
        {
            var query = (IQueryable<TEntity>)_context.Set<TEntity>();

            if (excludeDeleted)
                query = query.Where(x => x.DeletionDate == null);

            if (predicate != null)
                query = query.Where(predicate);

            var retorno = await query.ToListAsync();
            return retorno;
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _context.Set<TEntity>()
                .FindAsync(id);
        }

        public virtual async Task<TEntity> GetByIdAsNoTrackingAsync(Guid id)
        {
            return await _context.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public void Delete(Guid id)
        {
            TEntity entity = null;
            Task.Run(async () => entity = await GetByIdAsync(id)).Wait();

            if (entity == null)
                throw new IndexOutOfRangeException("Id não encontrado");

            Delete(entity);
        }

        public void Delete(IEnumerable<Guid> ids)
        {
            List<TEntity> entities = new();
            TEntity entity = null;

            ids.ToList().ForEach((x) =>
            {
                Task.Run(async () =>
                {
                    entity = await GetByIdAsync(x);
                    entities.Add(entity);
                }).Wait();
            });

            if (!ValidateEntities(entities))
                throw new KeyNotFoundException("Id não encontrado");

            Delete(entities);
        }

        public Task<bool> HasAny(Guid id)
        {
            return _context
                .Set<TEntity>()
                .AnyAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<IEnumerable<TEntity>> GetPaginated(int page, int pageSize)
        {
            return await _context.Set<TEntity>().Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            List<TEntity> list = new();
            await Task.Run(() => { entities.ToList().ForEach(async x => list.Add(await AddAsync(x))); });
            return list;
        }

        protected virtual async Task UpdateAsync(TEntity entity)
        {
            InternalUpdate(entity);
            await _context.SaveChangesAsync();
        }

        protected virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => { entities.ToList().ForEach(async x => await UpdateAsync(x)); });
        }

        private async Task InternalAdd(TEntity entity)
        {
            await Task.Run(() =>
            {
                _context.Set<TEntity>().Add(entity);
            });
        }

        private void InternalUpdate(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        private void Delete(TEntity entity)
        {
            if (entity is IPhisicallyDeletable)
                _context.Set<TEntity>().Remove(entity);
            else
                entity.DeletionDate = DateTime.UtcNow;

            _context.SaveChanges();
        }

        private void Delete(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(x => Delete(x));
        }

        private static bool ValidateEntities(IEnumerable<TEntity> entities)
        {
            return !entities.Any(x => x == null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any
        }
    }
}
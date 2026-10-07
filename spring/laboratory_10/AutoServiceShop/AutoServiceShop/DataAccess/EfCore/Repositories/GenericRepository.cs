using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceShop.DataAccess.EfCore.Repositories
{
    /// <summary>
    /// Базовый обобщённый репозиторий, работающий с DbContext.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности EF.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _set;

        public GenericRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _set = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> Query() => _set.AsQueryable();

        public async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
        {
            return await _set.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(object id, CancellationToken ct = default)
        {
            return await _set.FindAsync(new[] { id }, ct);
        }

        public async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken ct = default)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return await _set.Where(predicate).ToListAsync(ct);
        }

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _set.AddAsync(entity, ct);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            await _set.AddRangeAsync(entities, ct);
        }

        public void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _set.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _set.RemoveRange(entities);
        }
    }
}


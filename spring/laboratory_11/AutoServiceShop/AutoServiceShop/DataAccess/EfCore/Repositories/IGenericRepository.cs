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
    /// Обобщённый интерфейс репозитория для сущностей EF.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности доменной модели EF.</typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> Query();

        Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);

        Task<TEntity?> GetByIdAsync(object id, CancellationToken ct = default);

        Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken ct = default);

        Task AddAsync(TEntity entity, CancellationToken ct = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

        void Update(TEntity entity);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);
    }
}


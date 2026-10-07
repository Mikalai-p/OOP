using System;
using System.Threading;
using System.Threading.Tasks;
using AutoServiceShop.DataAccess.EfCore.Entities;
using AutoServiceShop.DataAccess.EfCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceShop.DataAccess.EfCore.UnitOfWork
{
    /// <summary>
    /// Реализация Unit of Work на базе AutoServiceDbContext.
    /// Один контекст — одна единица работы (одна транзакция).
    /// </summary>
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AutoServiceDbContext _context;

        private IGenericRepository<EfProduct>? _products;
        private IGenericRepository<EfCategory>? _categories;
        private IGenericRepository<EfOrder>? _orders;

        private bool _disposed;

        public EfUnitOfWork(DbContextOptions<AutoServiceDbContext> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _context = new AutoServiceDbContext(options);
        }

        /// <summary>
        /// Делаем контекст доступным внутри сборки, чтобы существующий сервис данных
        /// мог постепенно мигрировать на работу через Unit of Work.
        /// </summary>
        internal AutoServiceDbContext Context => _context;

        public IGenericRepository<EfProduct> Products =>
            _products ??= new GenericRepository<EfProduct>(_context);

        public IGenericRepository<EfCategory> Categories =>
            _categories ??= new GenericRepository<EfCategory>(_context);

        public IGenericRepository<EfOrder> Orders =>
            _orders ??= new GenericRepository<EfOrder>(_context);

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}


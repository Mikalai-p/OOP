using System;
using System.Threading;
using System.Threading.Tasks;
using AutoServiceShop.DataAccess.EfCore.Entities;
using AutoServiceShop.DataAccess.EfCore.Repositories;

namespace AutoServiceShop.DataAccess.EfCore.UnitOfWork
{
    /// <summary>
    /// Интерфейс паттерна Unit of Work для каталога автосервиса.
    /// Объединяет несколько репозиториев и единую транзакцию.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<EfProduct> Products { get; }
        IGenericRepository<EfCategory> Categories { get; }
        IGenericRepository<EfOrder> Orders { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}


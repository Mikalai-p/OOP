using AutoServiceShop.DataAccess.EfCore.Entities;
using AutoServiceShop.DataAccess;
using AutoServiceShop.DataAccess.EfCore.Repositories;
using AutoServiceShop.DataAccess.EfCore.UnitOfWork;
using AutoServiceShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace AutoServiceShop.DataAccess.EfCore
{
    public class EfAutoServiceDataService : IAutoServiceDataService
    {
        private readonly string _connectionString;
        private readonly DbContextOptions<AutoServiceDbContext> _options;

        public EfAutoServiceDataService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            var enableDetailedErrors = string.Equals(
                ConfigurationManager.AppSettings["EfEnableDetailedErrors"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            var enableSensitiveDataLogging = string.Equals(
                ConfigurationManager.AppSettings["EfEnableSensitiveDataLogging"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            var builder = new DbContextOptionsBuilder<AutoServiceDbContext>()
                .UseSqlite(_connectionString);

            if (enableDetailedErrors)
                builder.EnableDetailedErrors();
            if (enableSensitiveDataLogging)
                builder.EnableSensitiveDataLogging();

            _options = builder.Options;

            using var db = CreateContext();
            db.Database.EnsureCreated();
        }

        private static List<string> ParseImagePaths(string? imagePaths)
        {
            if (string.IsNullOrEmpty(imagePaths))
                return new List<string>();

            return new List<string>(
                imagePaths.Split(';', StringSplitOptions.RemoveEmptyEntries));
        }

        private static string SerializeImagePaths(IEnumerable<string>? paths)
        {
            if (paths == null)
                return string.Empty;

            return string.Join(";", paths.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private EfUnitOfWork CreateUnitOfWork() => new EfUnitOfWork(_options);
        private AutoServiceDbContext CreateContext() => new AutoServiceDbContext(_options);

        private void EnsureDefaultCategoryExists(EfUnitOfWork uow)
        {
            var defaultCategory = uow.Categories.Query().SingleOrDefault(c => c.Id == 1);
            if (defaultCategory != null) return;

            // Используем репозиторий и единицу работы
            uow.Categories.AddAsync(new EfCategory
            {
                Id = 1,
                Name = "Без категории",
                Description = null
            }).GetAwaiter().GetResult();

            uow.SaveChanges();
        }

        private int GetOrCreateCategoryId(string? categoryName, EfUnitOfWork uow)
        {
            EnsureDefaultCategoryExists(uow);

            if (string.IsNullOrWhiteSpace(categoryName))
                return 1;

            var existing = uow.Categories.Query().SingleOrDefault(c => c.Name == categoryName);
            if (existing != null)
                return existing.Id;

            var created = new EfCategory
            {
                Name = categoryName,
                Description = null
            };
            uow.Categories.AddAsync(created).GetAwaiter().GetResult();
            uow.SaveChanges();
            return created.Id;
        }

        private Product MapToUiProduct(EfProduct ef)
        {
            var categoryName = ef.Category?.Name ?? "Без категории";

            return new Product
            {
                Id = ef.Id,
                ShortName = ef.ShortName,
                FullName = ef.FullName,
                Description = ef.Description,
                Category = categoryName,
                Price = ef.Price,
                Quantity = ef.Quantity,
                Rating = ef.Rating,
                Country = ef.Country,
                Discount = ef.Discount,
                InStock = ef.InStock,
                Manufacturer = ef.Manufacturer,
                Color = ef.Color,
                Size = ef.Size,
                ImagePaths = ParseImagePaths(ef.ImagePaths)
            };
        }

        public ObservableCollection<Product> GetAllProducts()
        {
            using var uow = CreateUnitOfWork();
            var entities = uow.Products.Query()
                .AsNoTracking()
                .Include(p => p.Category)
                .ToList();

            return new ObservableCollection<Product>(entities.Select(MapToUiProduct));
        }

        public Product? GetProductById(int productId)
        {
            using var uow = CreateUnitOfWork();
            var ef = uow.Products.Query()
                .AsNoTracking()
                .Include(p => p.Category)
                .SingleOrDefault(p => p.Id == productId);

            return ef == null ? null : MapToUiProduct(ef);
        }

        public ObservableCollection<Category> GetAllCategories()
        {
            using var uow = CreateUnitOfWork();
            var entities = uow.Categories.Query().AsNoTracking().ToList();

            return new ObservableCollection<Category>(
                entities.Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }));
        }

        public int AddCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new InvalidOperationException("Название категории не задано.");

            using var uow = CreateUnitOfWork();

            var ef = new EfCategory
            {
                Name = category.Name.Trim(),
                Description = category.Description
            };

            uow.Categories.AddAsync(ef).GetAwaiter().GetResult();
            uow.SaveChanges();

            return ef.Id;
        }

        public void UpdateCategory(Category category)
        {
            using var uow = CreateUnitOfWork();

            var ef = uow.Categories.Query().SingleOrDefault(c => c.Id == category.Id);
            if (ef == null)
                throw new InvalidOperationException("Категория не найдена.");

            ef.Name = category.Name.Trim();
            ef.Description = category.Description;
            uow.Categories.Update(ef);
            uow.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            using var uow = CreateUnitOfWork();

            var ef = uow.Categories.Query().SingleOrDefault(c => c.Id == categoryId);
            if (ef == null)
                return;

            uow.Categories.Remove(ef);
            uow.SaveChanges();
        }

        public ObservableCollection<Order> GetAllOrders()
        {
            using var uow = CreateUnitOfWork();
            var entities = uow.Orders.Query()
                .AsNoTracking()
                .ToList();

            return new ObservableCollection<Order>(
                entities.Select(o => new Order
                {
                    Id = o.Id,
                    ProductId = o.ProductId,
                    Quantity = o.Quantity,
                    OrderDate = o.OrderDate,
                    CustomerName = o.CustomerName
                }));
        }

        public int AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.ShortName))
                throw new InvalidOperationException("Краткое название обязательно.");

            using var uow = CreateUnitOfWork();

            var categoryId = GetOrCreateCategoryId(product.Category, uow);

            var ef = new EfProduct
            {
                ShortName = product.ShortName,
                FullName = product.FullName,
                Description = product.Description,
                CategoryId = categoryId,

                Price = product.Price,
                Quantity = product.Quantity,
                Rating = product.Rating,
                Country = product.Country,
                Discount = product.Discount,
                InStock = product.InStock,
                Manufacturer = product.Manufacturer,
                Color = product.Color,
                Size = product.Size,
                ImagePaths = SerializeImagePaths(product.ImagePaths)
            };

            uow.Products.AddAsync(ef).GetAwaiter().GetResult();
            uow.SaveChanges();

            return ef.Id;
        }

        public void UpdateProduct(Product product)
        {
            if (product.Id <= 0)
                throw new InvalidOperationException("Некорректный Id товара.");

            using var uow = CreateUnitOfWork();

            var ef = uow.Products.Query().SingleOrDefault(p => p.Id == product.Id);
            if (ef == null)
                throw new InvalidOperationException("Товар не найден.");

            ef.ShortName = product.ShortName;
            ef.FullName = product.FullName;
            ef.Description = product.Description;
            ef.CategoryId = GetOrCreateCategoryId(product.Category, uow);

            ef.Price = product.Price;
            ef.Quantity = product.Quantity;
            ef.Rating = product.Rating;
            ef.Country = product.Country;
            ef.Discount = product.Discount;
            ef.InStock = product.InStock;
            ef.Manufacturer = product.Manufacturer;
            ef.Color = product.Color;
            ef.Size = product.Size;
            ef.ImagePaths = SerializeImagePaths(product.ImagePaths);

            uow.Products.Update(ef);
            uow.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            using var uow = CreateUnitOfWork();

            var ef = uow.Products.Query().SingleOrDefault(p => p.Id == productId);
            if (ef == null)
                return;

            uow.Products.Remove(ef);
            uow.SaveChanges();
        }

        public void AddOrder(Order order)
        {
            using var uow = CreateUnitOfWork();

            var product = uow.Products.Query().SingleOrDefault(p => p.Id == order.ProductId);
            if (product == null)
                throw new InvalidOperationException("Товар не найден.");

            if (product.Quantity < order.Quantity)
                throw new InvalidOperationException($"Недостаточно товара на складе. Доступно: {product.Quantity}");

            product.Quantity -= order.Quantity;

            var efOrder = new EfOrder
            {
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName
            };

            uow.Orders.AddAsync(efOrder).GetAwaiter().GetResult();
            uow.Products.Update(product);
            uow.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            using var uow = CreateUnitOfWork();

            var efOrder = uow.Orders.Query().SingleOrDefault(o => o.Id == order.Id);
            if (efOrder == null)
                throw new InvalidOperationException("Заказ не найден.");

            var product = uow.Products.Query().SingleOrDefault(p => p.Id == efOrder.ProductId);
            if (product == null)
                throw new InvalidOperationException("Товар не найден.");

            // Положительная diff = возврат товара на склад
            int diff = efOrder.Quantity - order.Quantity;
            product.Quantity += diff;

            efOrder.Quantity = order.Quantity;
            efOrder.CustomerName = order.CustomerName;

            uow.Products.Update(product);
            uow.Orders.Update(efOrder);
            uow.SaveChanges();
        }

        public void DeleteOrder(int orderId)
        {
            using var uow = CreateUnitOfWork();

            var efOrder = uow.Orders.Query().SingleOrDefault(o => o.Id == orderId);
            if (efOrder == null)
                throw new InvalidOperationException("Заказ не найден.");

            var product = uow.Products.Query().SingleOrDefault(p => p.Id == efOrder.ProductId);
            if (product != null)
            {
                product.Quantity += efOrder.Quantity;
            }

            uow.Products.Update(product!);
            uow.Orders.Remove(efOrder);
            uow.SaveChanges();
        }

        public void ResetOrdersIdCounter()
        {
            using var uow = CreateUnitOfWork();
            uow.Context.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='Orders'");
        }

        public async Task<ObservableCollection<Product>> QueryProductsAsync(
            string? searchText,
            string? selectedCategory,
            decimal minPrice,
            decimal maxPrice,
            ProductSortOrder sortOrder,
            CancellationToken ct = default)
        {
            using var uow = CreateUnitOfWork();

            var query = uow.Products.Query()
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var text = searchText.Trim();
                query = query.Where(p =>
                    p.ShortName.Contains(text) ||
                    (p.Description != null && p.Description.Contains(text)));
            }

            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == selectedCategory);
            }

            if (minPrice > 0)
                query = query.Where(p => p.Price >= minPrice);
            if (maxPrice > 0)
                query = query.Where(p => p.Price <= maxPrice);

            query = sortOrder switch
            {
                ProductSortOrder.PriceAscending => query.OrderBy(p => p.Price),
                ProductSortOrder.PriceDescending => query.OrderByDescending(p => p.Price),
                _ => query
            };

            var list = await query.ToListAsync(ct);
            return new ObservableCollection<Product>(list.Select(MapToUiProduct));
        }

        public async Task<ObservableCollection<Product>> QueryProductsByShortNameAsync(
            string? searchText,
            string? selectedCategory,
            decimal minPrice,
            decimal maxPrice,
            ProductSortOrder sortOrder,
            CancellationToken ct = default)
        {
            using var uow = CreateUnitOfWork();

            var query = uow.Products.Query()
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var text = searchText.Trim();
                query = query.Where(p => p.ShortName.Contains(text));
            }

            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == selectedCategory);
            }

            if (minPrice > 0)
                query = query.Where(p => p.Price >= minPrice);
            if (maxPrice > 0)
                query = query.Where(p => p.Price <= maxPrice);

            query = sortOrder switch
            {
                ProductSortOrder.PriceAscending => query.OrderBy(p => p.Price),
                ProductSortOrder.PriceDescending => query.OrderByDescending(p => p.Price),
                _ => query
            };

            var list = await query.ToListAsync(ct);
            return new ObservableCollection<Product>(list.Select(MapToUiProduct));
        }
    }
}


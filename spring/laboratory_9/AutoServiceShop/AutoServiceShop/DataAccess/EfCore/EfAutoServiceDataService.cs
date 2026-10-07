using AutoServiceShop.DataAccess.EfCore.Entities;
using AutoServiceShop.DataAccess;
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

        private AutoServiceDbContext CreateContext() => new AutoServiceDbContext(_options);

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

        private AutoServiceDbContext DefaultContext() => CreateContext();

        private void EnsureDefaultCategoryExists(AutoServiceDbContext db)
        {
            var defaultCategory = db.Categories.SingleOrDefault(c => c.Id == 1);
            if (defaultCategory != null) return;

            db.Categories.Add(new EfCategory
            {
                Id = 1,
                Name = "Без категории",
                Description = null
            });
            db.SaveChanges();
        }

        private int GetOrCreateCategoryId(string? categoryName, AutoServiceDbContext db)
        {
            EnsureDefaultCategoryExists(db);

            if (string.IsNullOrWhiteSpace(categoryName))
                return 1;

            var existing = db.Categories.SingleOrDefault(c => c.Name == categoryName);
            if (existing != null)
                return existing.Id;

            var created = new EfCategory
            {
                Name = categoryName,
                Description = null
            };
            db.Categories.Add(created);
            db.SaveChanges();
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
            using var db = DefaultContext();
            var entities = db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .ToList();

            return new ObservableCollection<Product>(entities.Select(MapToUiProduct));
        }

        public Product? GetProductById(int productId)
        {
            using var db = DefaultContext();
            var ef = db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .SingleOrDefault(p => p.Id == productId);

            return ef == null ? null : MapToUiProduct(ef);
        }

        public ObservableCollection<Category> GetAllCategories()
        {
            using var db = DefaultContext();
            var entities = db.Categories.AsNoTracking().ToList();

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

            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var ef = new EfCategory
            {
                Name = category.Name.Trim(),
                Description = category.Description
            };
            db.Categories.Add(ef);
            db.SaveChanges();
            tx.Commit();

            return ef.Id;
        }

        public void UpdateCategory(Category category)
        {
            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var ef = db.Categories.SingleOrDefault(c => c.Id == category.Id);
            if (ef == null)
                throw new InvalidOperationException("Категория не найдена.");

            ef.Name = category.Name.Trim();
            ef.Description = category.Description;
            db.SaveChanges();

            tx.Commit();
        }

        public void DeleteCategory(int categoryId)
        {
            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var ef = db.Categories.SingleOrDefault(c => c.Id == categoryId);
            if (ef == null)
                return;

            db.Categories.Remove(ef);
            db.SaveChanges();

            tx.Commit();
        }

        public ObservableCollection<Order> GetAllOrders()
        {
            using var db = DefaultContext();
            var entities = db.Orders
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

            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var categoryId = GetOrCreateCategoryId(product.Category, db);

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

            db.Products.Add(ef);
            db.SaveChanges();
            tx.Commit();

            return ef.Id;
        }

        public void UpdateProduct(Product product)
        {
            if (product.Id <= 0)
                throw new InvalidOperationException("Некорректный Id товара.");

            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var ef = db.Products.SingleOrDefault(p => p.Id == product.Id);
            if (ef == null)
                throw new InvalidOperationException("Товар не найден.");

            ef.ShortName = product.ShortName;
            ef.FullName = product.FullName;
            ef.Description = product.Description;
            ef.CategoryId = GetOrCreateCategoryId(product.Category, db);

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

            db.SaveChanges();
            tx.Commit();
        }

        public void DeleteProduct(int productId)
        {
            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var ef = db.Products.SingleOrDefault(p => p.Id == productId);
            if (ef == null)
                return;

            db.Products.Remove(ef);
            db.SaveChanges();
            tx.Commit();
        }

        public void AddOrder(Order order)
        {
            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var product = db.Products.SingleOrDefault(p => p.Id == order.ProductId);
            if (product == null)
                throw new InvalidOperationException("Товар не найден.");

            if (product.Quantity < order.Quantity)
                throw new InvalidOperationException($"Недостаточно товара на складе. Доступно: {product.Quantity}");

            product.Quantity -= order.Quantity;

            db.Orders.Add(new EfOrder
            {
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName
            });

            db.SaveChanges();
            tx.Commit();
        }

        public void UpdateOrder(Order order)
        {
            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var efOrder = db.Orders.SingleOrDefault(o => o.Id == order.Id);
            if (efOrder == null)
                throw new InvalidOperationException("Заказ не найден.");

            var product = db.Products.SingleOrDefault(p => p.Id == efOrder.ProductId);
            if (product == null)
                throw new InvalidOperationException("Товар не найден.");

            // Положительная diff = возврат товара на склад
            int diff = efOrder.Quantity - order.Quantity;
            product.Quantity += diff;

            efOrder.Quantity = order.Quantity;
            efOrder.CustomerName = order.CustomerName;

            db.SaveChanges();
            tx.Commit();
        }

        public void DeleteOrder(int orderId)
        {
            using var db = DefaultContext();
            using var tx = db.Database.BeginTransaction();

            var efOrder = db.Orders.SingleOrDefault(o => o.Id == orderId);
            if (efOrder == null)
                throw new InvalidOperationException("Заказ не найден.");

            var product = db.Products.SingleOrDefault(p => p.Id == efOrder.ProductId);
            if (product != null)
            {
                product.Quantity += efOrder.Quantity;
            }

            db.Orders.Remove(efOrder);
            db.SaveChanges();

            tx.Commit();
        }

        public void ResetOrdersIdCounter()
        {
            using var db = DefaultContext();
            db.Database.ExecuteSqlRaw("DELETE FROM sqlite_sequence WHERE name='Orders'");
        }

        public async Task<ObservableCollection<Product>> QueryProductsAsync(
            string? searchText,
            string? selectedCategory,
            decimal minPrice,
            decimal maxPrice,
            ProductSortOrder sortOrder,
            CancellationToken ct = default)
        {
            using var db = DefaultContext();

            var query = db.Products
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
            using var db = DefaultContext();

            var query = db.Products
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


using AutoServiceShop.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AutoServiceShop.DataAccess
{
    public enum ProductSortOrder
    {
        None = 0,
        PriceAscending = 1,
        PriceDescending = 2
    }

    public interface IAutoServiceDataService
    {
        // CRUD: Products
        ObservableCollection<Product> GetAllProducts();
        Product? GetProductById(int productId);
        Task<ObservableCollection<Product>> QueryProductsAsync(
            string? searchText,
            string? selectedCategory,
            decimal minPrice,
            decimal maxPrice,
            ProductSortOrder sortOrder,
            CancellationToken ct = default);
        Task<ObservableCollection<Product>> QueryProductsByShortNameAsync(
            string? searchText,
            string? selectedCategory,
            decimal minPrice,
            decimal maxPrice,
            ProductSortOrder sortOrder,
            CancellationToken ct = default);

        int AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);

        // CRUD: Categories
        ObservableCollection<Category> GetAllCategories();
        int AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(int categoryId);

        // CRUD: Orders
        ObservableCollection<Order> GetAllOrders();
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int orderId);
        void ResetOrdersIdCounter();
    }
}


using AutoServiceShop.Models;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AutoServiceShop.DataAccess
{
    public class SQLiteDataService
    {
        private readonly string _connectionString;

        public SQLiteDataService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        private List<string> ParseImagePaths(string imagePaths)
        {
            if (string.IsNullOrEmpty(imagePaths))
                return new List<string>();
            return new List<string>(imagePaths.Split(';', StringSplitOptions.RemoveEmptyEntries));
        }

        private string SerializeImagePaths(List<string> paths)
        {
            if (paths == null || paths.Count == 0)
                return string.Empty;
            return string.Join(";", paths);
        }

        public ObservableCollection<Product> GetAllProducts()
        {
            var products = new ObservableCollection<Product>();
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            var query = @"
        SELECT 
            p.Id,
            p.ShortName,
            p.FullName,
            p.Description,
            p.CategoryId,
            p.Price,
            p.Quantity,
            p.Rating,
            p.Country,
            p.Discount,
            p.InStock,
            p.Manufacturer,
            p.Color,
            p.Size,
            p.ImagePaths,
            c.Name as CategoryName
        FROM Products p
        LEFT JOIN Categories c ON p.CategoryId = c.Id";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var product = new Product
                {
                    Id = reader.GetInt32(0),
                    ShortName = reader.GetString(1),
                    FullName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Category = reader.IsDBNull(15) ? "Без категории" : reader.GetString(15),
                    Price = reader.GetDecimal(5),
                    Quantity = reader.GetInt32(6),
                    Rating = reader.IsDBNull(7) ? 0 : reader.GetDouble(7),
                    Country = reader.IsDBNull(8) ? null : reader.GetString(8),
                    Discount = reader.IsDBNull(9) ? 0 : reader.GetDouble(9),
                    InStock = reader.GetInt32(10) == 1,
                    Manufacturer = reader.IsDBNull(11) ? null : reader.GetString(11),
                    Color = reader.IsDBNull(12) ? null : reader.GetString(12),
                    Size = reader.IsDBNull(13) ? null : reader.GetString(13),
                    ImagePaths = ParseImagePaths(reader.IsDBNull(14) ? null : reader.GetString(14))
                };
                products.Add(product);
            }
            return products;
        }

        public async Task<ObservableCollection<Product>> GetAllProductsAsync()
        {
            return await Task.Run(() => GetAllProducts());
        }

        public Product GetProductById(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            var query = @"
                SELECT p.*, c.Name as CategoryName 
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE p.Id = @id";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    Id = reader.GetInt32(0),
                    ShortName = reader.GetString(1),
                    FullName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Category = reader.IsDBNull(15) ? "Без категории" : reader.GetString(15),
                    Price = reader.GetDecimal(5),
                    Quantity = reader.GetInt32(6),
                    Rating = reader.IsDBNull(7) ? 0 : reader.GetDouble(7),
                    Country = reader.IsDBNull(8) ? null : reader.GetString(8),
                    Discount = reader.IsDBNull(9) ? 0 : reader.GetDouble(9),
                    InStock = reader.GetInt32(10) == 1,
                    Manufacturer = reader.IsDBNull(11) ? null : reader.GetString(11),
                    Color = reader.IsDBNull(12) ? null : reader.GetString(12),
                    Size = reader.IsDBNull(13) ? null : reader.GetString(13),
                    ImagePaths = ParseImagePaths(reader.IsDBNull(14) ? null : reader.GetString(14))
                };
            }
            return null;
        }

        public int AddProduct(Product product)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                int categoryId = GetOrCreateCategoryId(product.Category, connection, transaction);
                var query = @"
                    INSERT INTO Products (
                        ShortName, FullName, Description, CategoryId, Price, Quantity, 
                        Rating, Country, Discount, InStock, Manufacturer, Color, Size, ImagePaths
                    ) VALUES (
                        @ShortName, @FullName, @Description, @CategoryId, @Price, @Quantity, 
                        @Rating, @Country, @Discount, @InStock, @Manufacturer, @Color, @Size, @ImagePaths
                    );
                    SELECT last_insert_rowid();";

                using var command = new SQLiteCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@ShortName", product.ShortName);
                command.Parameters.AddWithValue("@FullName", (object)product.FullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Quantity", product.Quantity);
                command.Parameters.AddWithValue("@Rating", product.Rating);
                command.Parameters.AddWithValue("@Country", (object)product.Country ?? DBNull.Value);
                command.Parameters.AddWithValue("@Discount", product.Discount);
                command.Parameters.AddWithValue("@InStock", product.InStock ? 1 : 0);
                command.Parameters.AddWithValue("@Manufacturer", (object)product.Manufacturer ?? DBNull.Value);
                command.Parameters.AddWithValue("@Color", (object)product.Color ?? DBNull.Value);
                command.Parameters.AddWithValue("@Size", (object)product.Size ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImagePaths", SerializeImagePaths(product.ImagePaths));

                int newId = Convert.ToInt32(command.ExecuteScalar());
                transaction.Commit();
                return newId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdateProduct(Product product)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                int categoryId = GetOrCreateCategoryId(product.Category, connection, transaction);
                var query = @"
                    UPDATE Products SET
                        ShortName = @ShortName,
                        FullName = @FullName,
                        Description = @Description,
                        CategoryId = @CategoryId,
                        Price = @Price,
                        Quantity = @Quantity,
                        Rating = @Rating,
                        Country = @Country,
                        Discount = @Discount,
                        InStock = @InStock,
                        Manufacturer = @Manufacturer,
                        Color = @Color,
                        Size = @Size,
                        ImagePaths = @ImagePaths
                    WHERE Id = @Id";

                using var command = new SQLiteCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@Id", product.Id);
                command.Parameters.AddWithValue("@ShortName", product.ShortName);
                command.Parameters.AddWithValue("@FullName", (object)product.FullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Quantity", product.Quantity);
                command.Parameters.AddWithValue("@Rating", product.Rating);
                command.Parameters.AddWithValue("@Country", (object)product.Country ?? DBNull.Value);
                command.Parameters.AddWithValue("@Discount", product.Discount);
                command.Parameters.AddWithValue("@InStock", product.InStock ? 1 : 0);
                command.Parameters.AddWithValue("@Manufacturer", (object)product.Manufacturer ?? DBNull.Value);
                command.Parameters.AddWithValue("@Color", (object)product.Color ?? DBNull.Value);
                command.Parameters.AddWithValue("@Size", (object)product.Size ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImagePaths", SerializeImagePaths(product.ImagePaths));

                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void DeleteProduct(int productId)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var query = "DELETE FROM Products WHERE Id = @Id";
                using var command = new SQLiteCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@Id", productId);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public ObservableCollection<Category> GetAllCategories()
        {
            var categories = new ObservableCollection<Category>();
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            var query = "SELECT Id, Name, Description FROM Categories";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return categories;
        }

        private int GetOrCreateCategoryId(string categoryName, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return 1;

            var selectQuery = "SELECT Id FROM Categories WHERE Name = @name";
            using var selectCmd = new SQLiteCommand(selectQuery, connection, transaction);
            selectCmd.Parameters.AddWithValue("@name", categoryName);
            var result = selectCmd.ExecuteScalar();
            if (result != null)
                return Convert.ToInt32(result);

            var insertQuery = "INSERT INTO Categories (Name) VALUES (@name); SELECT last_insert_rowid();";
            using var insertCmd = new SQLiteCommand(insertQuery, connection, transaction);
            insertCmd.Parameters.AddWithValue("@name", categoryName);
            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }

        public void AddOrder(Order order)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Проверяем, достаточно ли товара
                var checkQuery = "SELECT Quantity FROM Products WHERE Id = @ProductId";
                using var checkCmd = new SQLiteCommand(checkQuery, connection, transaction);
                checkCmd.Parameters.AddWithValue("@ProductId", order.ProductId);
                var currentQty = Convert.ToInt32(checkCmd.ExecuteScalar());

                // Разрешаем заказ последнего товара (Quantity == currentQty)
                if (currentQty < order.Quantity)
                    throw new InvalidOperationException($"Недостаточно товара на складе. Доступно: {currentQty}");

                // Уменьшаем количество товара
                var updateQuery = "UPDATE Products SET Quantity = Quantity - @Quantity WHERE Id = @ProductId";
                using var updateCmd = new SQLiteCommand(updateQuery, connection, transaction);
                updateCmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                updateCmd.Parameters.AddWithValue("@ProductId", order.ProductId);
                updateCmd.ExecuteNonQuery();

                // Добавляем заказ
                var insertQuery = @"
            INSERT INTO Orders (ProductId, Quantity, OrderDate, CustomerName)
            VALUES (@ProductId, @Quantity, @OrderDate, @CustomerName)";
                using var insertCmd = new SQLiteCommand(insertQuery, connection, transaction);
                insertCmd.Parameters.AddWithValue("@ProductId", order.ProductId);
                insertCmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                insertCmd.Parameters.AddWithValue("@OrderDate", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
                insertCmd.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                insertCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        // Добавить свойство для доступа к connectionString
        internal string ConnectionString => _connectionString;
        public ObservableCollection<Order> GetAllOrders()
        {
            var orders = new ObservableCollection<Order>();
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            var query = "SELECT Id, ProductId, Quantity, OrderDate, CustomerName FROM Orders";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new Order
                {
                    Id = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    Quantity = reader.GetInt32(2),
                    OrderDate = DateTime.Parse(reader.GetString(3)),
                    CustomerName = reader.GetString(4)
                });
            }
            return orders;
        }

        public void DeleteOrder(int orderId)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var selectQuery = "SELECT ProductId, Quantity FROM Orders WHERE Id = @OrderId";
                using var selectCmd = new SQLiteCommand(selectQuery, connection, transaction);
                selectCmd.Parameters.AddWithValue("@OrderId", orderId);
                using var reader = selectCmd.ExecuteReader();
                if (!reader.Read())
                    throw new InvalidOperationException("Заказ не найден.");
                int productId = reader.GetInt32(0);
                int quantity = reader.GetInt32(1);

                var updateQuery = "UPDATE Products SET Quantity = Quantity + @Quantity WHERE Id = @ProductId";
                using var updateCmd = new SQLiteCommand(updateQuery, connection, transaction);
                updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                updateCmd.Parameters.AddWithValue("@ProductId", productId);
                updateCmd.ExecuteNonQuery();

                var deleteQuery = "DELETE FROM Orders WHERE Id = @OrderId";
                using var deleteCmd = new SQLiteCommand(deleteQuery, connection, transaction);
                deleteCmd.Parameters.AddWithValue("@OrderId", orderId);
                deleteCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void UpdateOrder(Order order)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Получаем текущий заказ из БД
                var selectQuery = "SELECT ProductId, Quantity FROM Orders WHERE Id = @OrderId";
                using var selectCmd = new SQLiteCommand(selectQuery, connection, transaction);
                selectCmd.Parameters.AddWithValue("@OrderId", order.Id);
                using var reader = selectCmd.ExecuteReader();
                if (!reader.Read())
                    throw new InvalidOperationException("Заказ не найден.");

                int productId = reader.GetInt32(0);
                int oldQuantity = reader.GetInt32(1);
                reader.Close();

                // Если количество изменилось
                if (oldQuantity != order.Quantity)
                {
                    int quantityDiff = oldQuantity - order.Quantity; // положительная разница = возврат товара

                    // Обновляем количество товара
                    var updateProductQuery = "UPDATE Products SET Quantity = Quantity + @diff WHERE Id = @ProductId";
                    using var updateProductCmd = new SQLiteCommand(updateProductQuery, connection, transaction);
                    updateProductCmd.Parameters.AddWithValue("@diff", quantityDiff);
                    updateProductCmd.Parameters.AddWithValue("@ProductId", productId);
                    updateProductCmd.ExecuteNonQuery();
                }

                // Обновляем заказ
                var updateOrderQuery = @"
            UPDATE Orders SET 
                Quantity = @Quantity,
                CustomerName = @CustomerName
            WHERE Id = @Id";
                using var updateOrderCmd = new SQLiteCommand(updateOrderQuery, connection, transaction);
                updateOrderCmd.Parameters.AddWithValue("@Id", order.Id);
                updateOrderCmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                updateOrderCmd.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                updateOrderCmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void ResetOrdersIdCounter()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            var cmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name='Orders'", connection);
            cmd.ExecuteNonQuery();
        }
    }
}
using System.Data.SQLite;
using System;
using System.Windows;

namespace AutoServiceShop.DataAccess
{
    public static class DatabaseInitializer
    {
        private static readonly string ConnectionString;

        static DatabaseInitializer()
        {
            try
            {
                ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AutoServiceDb"]?.ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка чтения конфигурации: {ex.Message}\n\nПриложение будет работать без базы данных.",
                                "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectionString = null;
            }
        }

        public static void EnsureDatabaseCreated()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return;

            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Categories (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Description TEXT)", connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShortName TEXT NOT NULL,
                    FullName TEXT,
                    Description TEXT,
                    CategoryId INTEGER,
                    Price DECIMAL(10,2) NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Rating REAL,
                    Country TEXT,
                    Discount REAL,
                    InStock INTEGER,
                    Manufacturer TEXT,
                    Color TEXT,
                    Size TEXT,
                    ImagePaths TEXT,
                    FOREIGN KEY(CategoryId) REFERENCES Categories(Id))", connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    OrderDate TEXT NOT NULL,
                    CustomerName TEXT,
                    FOREIGN KEY(ProductId) REFERENCES Products(Id))", connection))
                    cmd.ExecuteNonQuery();

                SeedData(connection);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void SeedData(SQLiteConnection connection)
        {
            try
            {
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Categories", connection))
                {
                    if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
                    {
                        using (var insert = new SQLiteCommand("INSERT INTO Categories (Name) VALUES ('Услуги'), ('Запчасти'), ('Аксессуары')", connection))
                            insert.ExecuteNonQuery();
                    }
                }

                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Products", connection))
                {
                    if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
                    {
                        using (var insert = new SQLiteCommand(@"
                            INSERT INTO Products (ShortName, FullName, Description, CategoryId, Price, Quantity, Rating, Country, Discount, InStock, Manufacturer, ImagePaths) VALUES
                            ('Замена масла', 'Замена моторного масла и масляного фильтра', 'Профессиональная замена масла', 1, 2500, 0, 4.8, 'Россия', 0, 1, 'Автосервис', 'Images/oil_change.jpg'),
                            ('Тормозные колодки', 'Комплект передних тормозных колодок TRW', 'Оригинальные колодки', 2, 3200, 15, 4.7, 'Германия', 5, 1, 'TRW', 'Images/brake_pads.jpg')", connection))
                            insert.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка заполнения данными: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
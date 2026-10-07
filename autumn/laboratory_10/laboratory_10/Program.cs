using System;
using System.Collections.Generic;
using System.Linq;

public class Product : IComparable<Product>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Manufacturer { get; set; }
    public int Quantity { get; set; }

    public int CompareTo(Product other) => Price.CompareTo(other.Price);

    public override string ToString() =>
        $"{Name} (Manufacturer: {Manufacturer}, Price: {Price}, Quantity: {Quantity})";
}

public class Manufacturer
{
    public string Name { get; set; }
    public string Country { get; set; }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== 1. ЗАПРОСЫ К МАССИВУ МЕСЯЦЕВ ===");

        string[] months = {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        int n = 5;

        var lengthN = months.Where(m => m.Length == n);
        Console.WriteLine($"Месяцы с длиной {n}: {string.Join(", ", lengthN)}");

        var summerWinter = months.Where(m =>
            m == "June" || m == "July" || m == "August" ||
            m == "December" || m == "January" || m == "February");
        Console.WriteLine($"Летние и зимние месяцы: {string.Join(", ", summerWinter)}");

        var alphabetical = months.OrderBy(m => m);
        Console.WriteLine($"Месяцы в алфавитном порядке: {string.Join(", ", alphabetical)}");

        var withU = months.Where(m => m.Contains('u') && m.Length >= 4);
        Console.WriteLine($"Месяцы с буквой 'u' и длиной >= 4: {string.Join(", ", withU)}");

        Console.WriteLine("\n=== 2. КОЛЛЕКЦИЯ ПРОДУКТОВ ===");

        var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 1200, Manufacturer = "Dell", Quantity = 5 },
            new Product { Name = "Mouse", Price = 25, Manufacturer = "Logitech", Quantity = 10 },
            new Product { Name = "Keyboard", Price = 75, Manufacturer = "Logitech", Quantity = 8 },
            new Product { Name = "Monitor", Price = 300, Manufacturer = "Dell", Quantity = 3 },
            new Product { Name = "Laptop", Price = 1500, Manufacturer = "HP", Quantity = 2 },
            new Product { Name = "Tablet", Price = 800, Manufacturer = "Samsung", Quantity = 6 },
            new Product { Name = "Phone", Price = 1000, Manufacturer = "Apple", Quantity = 12 },
            new Product { Name = "Headphones", Price = 100, Manufacturer = "Sony", Quantity = 15 },
            new Product { Name = "Laptop", Price = 900, Manufacturer = "Acer", Quantity = 4 },
            new Product { Name = "Camera", Price = 700, Manufacturer = "Canon", Quantity = 7 }
        };

        var expensiveProducts = from p in products
                                where p.Price > 100
                                select p;
        Console.WriteLine("Товары с ценой > 100:");
        foreach (var p in expensiveProducts)
            Console.WriteLine($"  {p}");

        Console.WriteLine("\n=== 3. ВАРИАНТНЫЕ ЗАПРОСЫ ===");

        var laptops = products.Where(p => p.Name == "Laptop");
        Console.WriteLine("Все ноутбуки:");
        foreach (var laptop in laptops)
            Console.WriteLine($"  {laptop}");

        decimal maxPrice = 1000;
        var affordableLaptops = products.Where(p => p.Name == "Laptop" && p.Price <= maxPrice);
        Console.WriteLine($"\nНоутбуки с ценой <= {maxPrice}:");
        foreach (var laptop in affordableLaptops)
            Console.WriteLine($"  {laptop}");

        int countExpensive = products.Select(p => p.Name)
                                   .Distinct()
                                   .Count(n => products.Any(p => p.Name == n && p.Price > 100));
        Console.WriteLine($"\nКоличество наименований с ценой > 100: {countExpensive}");

        var maxProduct = products.OrderByDescending(p => p.Price).First();
        Console.WriteLine($"\nСамый дорогой товар: {maxProduct}");

        var sortedProducts = products.OrderBy(p => p.Manufacturer).ThenBy(p => p.Quantity);
        Console.WriteLine("\nТовары, отсортированные по производителю и количеству:");
        foreach (var product in sortedProducts)
            Console.WriteLine($"  {product}");

        Console.WriteLine("\n=== 4. СЛОЖНЫЙ ЗАПРОС ===");

        var complexQuery = products
            .Where(p => p.Price > 50)                          
            .OrderByDescending(p => p.Price)                   
            .GroupBy(p => p.Manufacturer)                      
            .Select(g => new
            {
                Manufacturer = g.Key,
                TotalQuantity = g.Sum(p => p.Quantity),        // Агрегирование: сумма количества
                ExpensiveProducts = g.Where(p => p.Price > 500) 
            })
            .Where(x => x.TotalQuantity > 10)                  // Условие на агрегированное значение
            .Take(3)                                           
            .SelectMany(x => x.ExpensiveProducts);             

        Console.WriteLine("Результаты сложного запроса:");
        foreach (var p in complexQuery)
            Console.WriteLine($"  {p}");

        Console.WriteLine("\n=== 5. ЗАПРОС С JOIN ===");

        var manufacturers = new List<Manufacturer>
        {
            new Manufacturer { Name = "Dell", Country = "USA" },
            new Manufacturer { Name = "Logitech", Country = "Switzerland" },
            new Manufacturer { Name = "HP", Country = "USA" },
            new Manufacturer { Name = "Samsung", Country = "South Korea" },
            new Manufacturer { Name = "Apple", Country = "USA" },
            new Manufacturer { Name = "Sony", Country = "Japan" },
            new Manufacturer { Name = "Acer", Country = "Taiwan" },
            new Manufacturer { Name = "Canon", Country = "Japan" }
        };

        var joinedQuery = products
            .Join(manufacturers,
                  p => p.Manufacturer,
                  m => m.Name,
                  (p, m) => new {
                      ProductName = p.Name,
                      Price = p.Price,
                      Manufacturer = m.Name,
                      Country = m.Country
                  });

        Console.WriteLine("Товары со странами производителей:");
        foreach (var item in joinedQuery)
            Console.WriteLine($"  {item.ProductName} - {item.Price}$ - {item.Manufacturer} ({item.Country})");

        
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
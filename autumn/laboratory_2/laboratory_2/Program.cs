using System;
using System.Collections.Generic;
using System.Linq;

public partial class Product
{
    
    private readonly int id;
    private string name;
    private string upc;
    private string manufacturer;
    private decimal price;
    private int shelfLife;
    private int quantity;

    
    public const string Category = "General";

    
    private static int objectCount = 0;

    
    static Product()
    {
        Console.WriteLine("Статический конструктор вызван.");
    }

    
    private Product(string name, string manufacturer)
    {
        this.name = name;
        this.manufacturer = manufacturer;
        this.id = CalculateHash(name, manufacturer);
        objectCount++;
    }

    
    public Product() : this("Unknown", "Unknown") { }

    
    public Product(string name, string upc, string manufacturer, decimal price, int shelfLife, int quantity)
        : this(name, manufacturer)
    {
        this.upc = upc;
        this.price = price;
        this.shelfLife = shelfLife;
        this.quantity = quantity;
    }

    
    public Product(string name, string upc, string manufacturer, decimal price = 0, int shelfLife = 0)
        : this(name, manufacturer)
    {
        this.upc = upc;
        this.price = price;
        this.shelfLife = shelfLife;
        this.quantity = 0;
    }

    
    private static int CalculateHash(string name, string manufacturer)
    {
        return (name + manufacturer).GetHashCode();
    }

    
    public int Id => id;
    public string Name { get => name; set => name = value; }
    public string UPC { get => upc; set => upc = value; }
    public string Manufacturer { get => manufacturer; set => manufacturer = value; }
    public decimal Price { get => price; set => price = value; }
    public int ShelfLife { get => shelfLife; set => shelfLife = value; }
    public int Quantity { get => quantity; private set => quantity = value; } 

    
    public void UpdateQuantity(ref int newQuantity, out string status)
    {
        if (newQuantity >= 0)
        {
            quantity = newQuantity;
            status = "Успешно обновлено";
        }
        else
        {
            status = "Ошибка: отрицательное количество";
        }
    }

    
    public static void DisplayClassInfo()
    {
        Console.WriteLine($"Класс Product. Создано объектов: {objectCount}");
    }

    
    public decimal GetTotalValue() => price * quantity;

    
    public override bool Equals(object obj)
    {
        return obj is Product product && id == product.id;
    }

    public override int GetHashCode() => id;

    public override string ToString()
    {
        return $"ID: {id}, Наименование: {name}, UPC: {upc}, Производитель: {manufacturer}, Цена: {price}, Срок: {shelfLife}, Количество: {quantity}";
    }
}



class Program
{
    static void Main()
    {
        // Создание объектов
        Product p1 = new Product("Молоко", "123456", "Компания А", 80, 14, 100);
        Product p2 = new Product("Хлеб", "789012", "Компания Б", 30, 5, 50);
        Product p3 = new Product("Сок", "345678", "Компания А", 120, 30, 75);

        
        int newQty = 200;
        p1.UpdateQuantity(ref newQty, out string status);
        Console.WriteLine(status);

        
        Console.WriteLine($"p1 equals p2: {p1.Equals(p2)}");
        Console.WriteLine($"Hash p1: {p1.GetHashCode()}");

        
        Product.DisplayClassInfo();

        
        Product[] products = { p1, p2, p3 };

        // Запросы:
        // a) Список товаров по наименованию
        string targetName = "Молоко";
        var byName = products.Where(p => p.Name == targetName).ToList();
        Console.WriteLine($"\nТовары с наименованием '{targetName}':");
        byName.ForEach(Console.WriteLine);

        // b) Товары по наименованию и цене
        decimal maxPrice = 100;
        var byNameAndPrice = products.Where(p => p.Name == targetName && p.Price <= maxPrice).ToList();
        Console.WriteLine($"\nТовары '{targetName}' с ценой <= {maxPrice}:");
        byNameAndPrice.ForEach(Console.WriteLine);

        
        var anonymousProduct = new { Name = "Вода", Price = 50 };
        Console.WriteLine($"\nАнонимный тип: {anonymousProduct.Name}, {anonymousProduct.Price}");
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AutoServiceShop.Models;

namespace AutoServiceShop.Data
{
    public static class DataService
    {
        private static readonly string FilePath = "products.json";

        public static List<Product> LoadProducts()
        {
            if (!File.Exists(FilePath))
                return GetSampleProducts();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Product>>(json);
        }

        public static void SaveProducts(List<Product> products)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(products, options);
            File.WriteAllText(FilePath, json);
        }

        private static List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    ShortName = "Замена масла",
                    FullName = "Замена моторного масла и масляного фильтра",
                    Description = "Профессиональная замена масла с использованием масла Liqui Moly",
                    Category = "Услуги",
                    Price = 2500,
                    Quantity = 0,
                    Rating = 4.8,
                    Country = "Россия",
                    InStock = true,
                    Manufacturer = "Автосервис",
                    ImagePaths = new List<string> { "Images/oil_change.jpg" }
                },
                new Product
                {
                    Id = 2,
                    ShortName = "Тормозные колодки",
                    FullName = "Комплект передних тормозных колодок TRW",
                    Description = "Оригинальные тормозные колодки",
                    Category = "Запчасти",
                    Price = 3200,
                    Quantity = 15,
                    Rating = 4.7,
                    Country = "Германия",
                    Discount = 5,
                    InStock = true,
                    Manufacturer = "TRW",
                    ImagePaths = new List<string> { "Images/brake_pads.jpg" }
                }
            };
        }
    }
}
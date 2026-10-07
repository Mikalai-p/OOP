using System.Collections.Generic;

namespace AutoServiceShop.DataAccess.EfCore.Entities
{
    public class EfProduct
    {
        public int Id { get; set; }

        public string ShortName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public EfCategory? Category { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Rating { get; set; }
        public string? Country { get; set; }
        public double Discount { get; set; }
        public bool InStock { get; set; }
        public string? Manufacturer { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }

        // Хранится в БД как строка через ';'
        public string? ImagePaths { get; set; }

        public ICollection<EfOrder> Orders { get; set; } = new List<EfOrder>();
    }
}


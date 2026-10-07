using System.Collections.Generic;

namespace AutoServiceShop.DataAccess.EfCore.Entities
{
    public class EfCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<EfProduct> Products { get; set; } = new List<EfProduct>();
    }
}


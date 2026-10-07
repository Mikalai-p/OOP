using System;

namespace AutoServiceShop.DataAccess.EfCore.Entities
{
    public class EfOrder
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public EfProduct? Product { get; set; }

        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}


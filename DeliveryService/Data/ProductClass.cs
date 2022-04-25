using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Data
{
    public class ProductClass
    {
        [Key]
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}

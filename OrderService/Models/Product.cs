using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public int Count { get; set; }


    }
}

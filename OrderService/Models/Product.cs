using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public int Count { get; set; }
    }
}
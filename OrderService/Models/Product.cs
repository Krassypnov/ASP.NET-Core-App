using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int Count { get; set; }


    }
}

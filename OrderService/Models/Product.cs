using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Product
    {
        [Key]
        public int Guid { get; set; }
        public string Name { get; set; }

    }
}

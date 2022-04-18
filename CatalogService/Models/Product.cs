using System.ComponentModel.DataAnnotations;

namespace CatalogService.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }


    }
}

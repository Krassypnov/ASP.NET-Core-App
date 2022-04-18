using System.ComponentModel.DataAnnotations;

namespace CatalogService.Models
{
    public class Brand
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string BrandName { get; set; }
    }
}

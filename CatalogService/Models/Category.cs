using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}

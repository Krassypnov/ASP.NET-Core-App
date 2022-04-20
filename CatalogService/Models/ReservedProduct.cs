using System.ComponentModel.DataAnnotations;

namespace CatalogService.Models
{
    public class ReservedProduct
    {
        [Key]
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Count { get; set; }


    }
}

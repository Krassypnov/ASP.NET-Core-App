using System.ComponentModel.DataAnnotations;


namespace OrderService.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string ClientAddress { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}

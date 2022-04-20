using System.ComponentModel.DataAnnotations;


namespace OrderService.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        
    }
}

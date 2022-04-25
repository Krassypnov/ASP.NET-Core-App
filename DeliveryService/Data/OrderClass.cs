using System.ComponentModel.DataAnnotations;


namespace DeliveryService.Data
{
    public class OrderClass
    {
        [Key]
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsConfirmedOrder { get; set; }
        public bool IsDelivery { get; set; }

        
    }
}

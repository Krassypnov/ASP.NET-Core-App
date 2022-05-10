

namespace Messages
{
    public class ReservedProductMsg
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Count { get; set; }


    }
}

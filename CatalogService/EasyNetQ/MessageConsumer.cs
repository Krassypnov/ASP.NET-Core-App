using EasyNetQ.AutoSubscribe;
using Messages;

namespace CatalogService.EasyNetQ
{
    public class MessageConsumer : IConsumeAsync<ReservedProductMsg>
    {
        //private readonly AppDbContext _db;

        public MessageConsumer()
        {
            //this._db = db;
        }

        [AutoSubscriberConsumer(SubscriptionId = "M3.Dequeue")]
        public async Task ConsumeAsync(ReservedProductMsg msg, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(msg.Count);
            Console.WriteLine(msg.OrderId);
            Console.WriteLine(msg.ProductId);

        }
    }
}

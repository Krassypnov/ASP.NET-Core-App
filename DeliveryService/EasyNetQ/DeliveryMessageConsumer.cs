using EasyNetQ.AutoSubscribe;
using Messages;
using DeliveryService.Models;

namespace DeliveryService.EasyNetQ
{
    public class DeliveryMessageConsumer : IConsumeAsync<DeliveryOrderMsg>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DeliveryMessageConsumer(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
        }

        [AutoSubscriberConsumer(SubscriptionId = "M2.Dequeue")]
        public async Task ConsumeAsync(DeliveryOrderMsg msg, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.DeliveryOrders.AddAsync(new DeliveryOrder { OrderId = msg.OrderId });
            await dbContext.SaveChangesAsync();
        }
    }
}

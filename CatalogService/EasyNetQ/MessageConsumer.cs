using EasyNetQ.AutoSubscribe;
using Messages;
using CatalogService.Models;

namespace CatalogService.EasyNetQ
{
    public class MessageConsumer : IConsumeAsync<ReservedProductMsg>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MessageConsumer(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory ?? throw new System.ArgumentNullException(nameof(scopeFactory));
        }

        [AutoSubscriberConsumer(SubscriptionId = "M3.Dequeue")]
        public async Task ConsumeAsync(ReservedProductMsg msg, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.ReservedProducts.AddAsync(new ReservedProduct { ProductId = msg.ProductId, OrderId = msg.OrderId, Count = msg.Count });
            await dbContext.SaveChangesAsync();
        }
    }
}

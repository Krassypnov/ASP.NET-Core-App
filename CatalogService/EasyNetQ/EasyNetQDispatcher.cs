using EasyNetQ.AutoSubscribe;
using EasyNetQ;

namespace CatalogService.EasyNetQ
{
    internal class EasyNetQDispatcher : IAutoSubscriberMessageDispatcher
    {
        private IServiceProvider ServiceProvider { get; }

        public EasyNetQDispatcher(IServiceProvider container) => ServiceProvider = container;

        void IAutoSubscriberMessageDispatcher.Dispatch<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken)
        {
            var consumer = ServiceProvider.GetRequiredService<TConsumer>();
            consumer.Consume(message, cancellationToken);
        }

        async Task IAutoSubscriberMessageDispatcher.DispatchAsync<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken)
        {
            var consumer = ServiceProvider.GetRequiredService<TConsumer>();
            await consumer.ConsumeAsync(message, cancellationToken).ConfigureAwait(false);
        }
    }
}

using Microsoft.Extensions.Hosting;

namespace OrderManager.Infrastructure.Messaging;

public class OrderExpirationConsumerHostedService(OrderExpirationConsumer consumer) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        consumer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        consumer.Dispose();
        return Task.CompletedTask;
    }
}

using MassTransit;
using PoCWebApp.Applications.DomainEventHandlers;

namespace PoCWebApp.Consumers
{
    public record OrderCreated(Guid OrderId, string CustomerName, decimal Amount);

    public class OrderCreatedConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        public Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            Task.Delay(2000);
            Console.WriteLine($"Order received: {context.Message.CustomerName} - RM{context.Message.Amount}");
            return Task.CompletedTask;
        }
    }
}

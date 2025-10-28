using MassTransit;
using PoCWebApp.Applications.DomainEventHandlers;

namespace PoCWebApp.Consumers
{
    public record OrderCreated(Guid OrderId, string CustomerName, decimal Amount);

    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            //Thread.Sleep(5000);
            Console.WriteLine($"Order received: {context.Message.CustomerName} - RM{context.Message.Amount}");
            return Task.CompletedTask;
        }
    }
}

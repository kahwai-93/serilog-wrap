using MediatR;
using MassTransit;
using PoCWebApp.Data;

namespace PoCWebApp.Applications.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // Publish to Outbox (transactional)
            await _publishEndpoint.Publish(new OrderCreatedIntegrationEvent(
                notification.OrderId,
                notification.CustomerName,
                notification.Amount
            ), cancellationToken);
        }
    }

    // Integration event (the one that goes to RabbitMQ)
    public record OrderCreatedIntegrationEvent(Guid OrderId, string CustomerName, decimal Amount);
}

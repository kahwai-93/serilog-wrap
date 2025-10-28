using MassTransit;
using MediatR;
using PoCWebApp.Applications.DomainEventHandlers;
using PoCWebApp.Consumers;
using PoCWebApp.Data;

namespace PoCWebApp.Applications.Commands
{
    public record CreateOrderCommand(string CustomerName, decimal Amount) : IRequest<Guid>;

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly ApplicationDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateOrderCommandHandler(ApplicationDbContext db, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(request.CustomerName, request.Amount);
            _db.Orders.Add(order);

            await _publishEndpoint.Publish(new OrderCreated(
                order.Id,
                order.CustomerName,
                order.Amount
            ), cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}

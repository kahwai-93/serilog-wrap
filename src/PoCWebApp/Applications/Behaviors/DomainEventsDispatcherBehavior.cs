using MassTransit;
using MediatR;
using PoCWebApp.Data;

namespace PoCWebApp.Applications.Behaviors
{
    public class DomainEventsDispatcherBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
          where TRequest : IRequest<TResponse>
    {
        private readonly ApplicationDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;  // ✅ use MassTransit here

        public DomainEventsDispatcherBehavior(ApplicationDbContext db, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            var entities = _db.ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            foreach (var entity in entities)
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (var domainEvent in events)
                {
                    await _publishEndpoint.Publish(domainEvent, cancellationToken);
                }
            }

            return response;
        }
    }
}

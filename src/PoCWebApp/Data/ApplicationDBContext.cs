using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace PoCWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Register MassTransit Outbox entities
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }

    public class Order : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public static Order Create(string customerName, decimal amount)
        {
            var order = new Order
            {
                CustomerName = customerName,
                Amount = amount
            };

            // Raise Domain Event
            order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.CustomerName, order.Amount));
            return order;
        }
    }

    public record OrderCreatedDomainEvent(Guid OrderId, string CustomerName, decimal Amount) : INotification;

    public abstract class BaseEntity
    {
        private readonly List<INotification> _domainEvents = new();

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}

using MassTransit;
using MassTransit.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PoCWebApp.Applications.Behaviors;
using PoCWebApp.Consumers;
using PoCWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventsDispatcherBehavior<,>));

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<OrderCreatedConsumer>(cfg =>
    {
        cfg.UseScheduledRedelivery(r =>
        {
            r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));
        });
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
    });

    //x.AddConfigureEndpointsCallback((context, name, cfg) =>
    //{
    //    cfg.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
    //});

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", 5672, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseDelayedMessageScheduler(); // required for scheduled redelivery
        cfg.ConfigureEndpoints(context);
    });

    //x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
    //{
    //    o.UsePostgres();
    //    //o.QueryDelay = TimeSpan.FromSeconds(10);
    //    //o.DuplicateDetectionWindow = TimeSpan.FromMinutes(1);
    //    o.UseBusOutbox();
    //});
});

builder.Services.AddMassTransitHostedService();

//builder.Services.AddHostedService<OutboxCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

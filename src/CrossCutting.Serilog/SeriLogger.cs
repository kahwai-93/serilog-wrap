using Destructurama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace Common.Serilog
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
           (context, configuration) =>
           {
               BaseConfigure(context, configuration, "logs", false);
           };

        private static Action<HostBuilderContext, LoggerConfiguration, string, bool> BaseConfigure =>
           (context, configuration, table, isSkippingLifetimeLog) =>
           {
               var conn = context.Configuration.GetConnectionString("LogConnectionString");

               IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
                {
                    { "id", new IdAutoIncrementColumnWriter() },
                    { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                    { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                    { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                    { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
                };

               configuration
                   .Destructure.JsonNetTypes()
                   .Enrich.FromLogContext()
                   .WriteTo.Console()
                   .WriteTo.PostgreSQL(conn
                        , table
                        , columnWriters
                        , needAutoCreateTable: true
                        , useCopy: true
                        , queueLimit: 3000
                        , batchSizeLimit: 40
                        , period: new TimeSpan(0, 0, 5)
                        , appConfiguration: context.Configuration)
                   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                   .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                   .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                   .MinimumLevel.Override("Yarp.ReverseProxy.Forwarder.HttpForwarder", LogEventLevel.Warning)
                   .Enrich.WithMachineName()
                   .Enrich.WithEnvironmentName()
                   .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                   .Enrich.WithSensitiveDataMasking(options =>
                   {
                       options.MaskingOperators.Clear();
                       options.MaskProperties.Add("password");
                       options.MaskProperties.Add("PasswordHash");
                   });

               if (isSkippingLifetimeLog)
               {
                   configuration
                   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning);
               }
           };

        public static IServiceCollection AddSerilogWrapper(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ISerilogWrapper, SerilogWrapper>();

            return services;
        }
    }
}

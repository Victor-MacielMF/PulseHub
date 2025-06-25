using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PulseHub.Consumer.Extensions;
using PulseHub.Consumer.MercadoLivre.Services;
using PulseHub.Infrastructure.Extensions;
using PulseHub.Infrastructure.Messaging.Settings;
using Serilog;
using System;

namespace PulseHub.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configuração do Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/consumer-log-.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting PulseHub.Consumer");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "PulseHub.Consumer terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // <- chave para ativar Serilog
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.Configure<RabbitMQSettings>(
                        configuration.GetSection("RabbitMQ"));

                    services.AddInfrastructure(configuration);
                    services.AddConsumerServices(configuration);

                    services.AddHostedService<MercadoLivreQueueConsumerService>();
                    //services.AddHostedService<Worker>();
                });
    }
}

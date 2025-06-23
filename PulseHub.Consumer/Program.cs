using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PulseHub.Consumer.Extensions;
using PulseHub.Consumer.MercadoLivre.Services;
using PulseHub.Infrastructure.Extensions;
using PulseHub.Infrastructure.Messaging.Settings;

namespace PulseHub.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    // Configura RabbitMQ Settings
                    services.Configure<RabbitMQSettings>(
                        configuration.GetSection("RabbitMQ"));

                    // Injeta dependências da Infrastructure e do Consumer
                    services.AddInfrastructure(configuration);
                    services.AddConsumerServices(configuration);

                    // Registra o Worker do Mercado Livre
                    services.AddHostedService<MercadoLivreQueueConsumerService>();
                    //services.AddHostedService<Worker>();
                });
    }
}

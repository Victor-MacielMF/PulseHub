using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting PulseHub.Consumer");
                CreateHostBuilder(args, configuration).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(configuration); // lê o "Urls" do appsettings.json

                    webBuilder.ConfigureServices((context, services) =>
                    {
                        services.Configure<RabbitMQSettings>(
                            configuration.GetSection("RabbitMQ"));

                        services.AddInfrastructure(configuration);
                        services.AddConsumerServices(configuration);

                        services.AddHealthChecks()
                                .AddRabbitMQ(rabbitConnectionString: configuration.GetSection("RabbitMQ")["ConnectionString"]);

                        services.AddHostedService<MercadoLivreQueueConsumerService>();
                    });

                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHealthChecks("/health");
                        });
                    });
                });
    }
}

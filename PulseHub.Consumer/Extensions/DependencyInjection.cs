using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PulseHub.Consumer.MercadoLivre.Handlers;
using PulseHub.Consumer.MercadoLivre.Messaging.Implementations;
using PulseHub.Consumer.MercadoLivre.Messaging.Interfaces;
using PulseHub.Infrastructure.Extensions;

namespace PulseHub.Consumer.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddConsumerServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Reaproveita os serviços da infrastructure (DbContext, RabbitMQ, Repositorios, etc)
            services.AddInfrastructure(configuration);

            // Registra serviços específicos do consumidor Mercado Livre
            services.AddScoped<IMercadoLivreMessageProcessor, MercadoLivreMessageProcessor>();
            services.AddScoped<MercadoLivreQueueMessageHandler>();

            return services;
        }
    }
}

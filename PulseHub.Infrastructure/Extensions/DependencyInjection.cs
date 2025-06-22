using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PulseHub.Domain.Interfaces;
using PulseHub.Domain.Messaging;
using PulseHub.Infrastructure.Data;
using PulseHub.Infrastructure.Messaging.Implementations;
using PulseHub.Infrastructure.Messaging.Interfaces;
using PulseHub.Infrastructure.Repositories;

namespace PulseHub.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<PulseHubDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISyncEventRepository, SyncEventRepository>();
            services.AddScoped<IQueueMessageRepository, QueueMessageRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Mensageria
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
            services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

            return services;
        }
    }
}

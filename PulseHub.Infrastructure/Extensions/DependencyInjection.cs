using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PulseHub.Domain.Interfaces;
using PulseHub.Infrastructure.Data;
using PulseHub.Infrastructure.Repositories;

namespace Pulsehub.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar o DbContext
            services.AddDbContext<PulseHubDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Aqui futuramente também registra repositórios, services, etc.
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISyncEventRepository, SyncEventRepository>();
            services.AddScoped<IQueueMessageRepository, QueueMessageRepository>();

            return services;
        }
    }
}

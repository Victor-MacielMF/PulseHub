using Microsoft.Extensions.DependencyInjection;
using PulseHub.Application.Services.Implementations;
using PulseHub.Application.Services.Interfaces;

namespace PulseHub.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Application Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISyncEventService, SyncEventService>();
            services.AddScoped<IQueueMessageService, QueueMessageService>();

            return services;
        }
    }
}

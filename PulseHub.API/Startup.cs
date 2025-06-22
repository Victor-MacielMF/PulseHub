using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PulseHub.API.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PulseHub.API.Middlewares;
using PulseHub.Application.DTOs;
using PulseHub.Application.Mappings.Profiles;
using PulseHub.Application.Services.Implementations;
using PulseHub.Application.Services.Interfaces;
using PulseHub.Infrastructure.Extensions;
using PulseHub.Infrastructure.Messaging.Settings;
using System.Linq;
using System.Reflection;

namespace PulseHub.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Dependências da Infrastructure
            services.AddInfrastructure(Configuration);

            // Dependências da Application (Application Services)
            services.AddApplicationServices();

            // Controllers
            services.AddControllers();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PulseHub API",
                    Version = "v1",
                    Description = "API para gerenciamento de produtos e sincronização com marketplaces.",
                    Contact = new OpenApiContact
                    {
                        Name = "João Victor Maciel de Freitas",
                        Email = "joaovictormacieldefreitas@gmail.com"
                    }
                });

                c.EnableAnnotations();
            });

            // AutoMapper
            services.AddAutoMapper(
                Assembly.GetExecutingAssembly(),   // API
                typeof(ProductProfile).Assembly    // Application
            );

            // Tratamento global de erros de ModelState (Validações)
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var response = new ApiResponse<object>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Validation failed",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            // Configuração do RabbitMQ Settings
            services.Configure<RabbitMQSettings>(
                Configuration.GetSection("RabbitMQ"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulseHub.API v1"));
            }

            app.UseHttpsRedirection();

            // Middleware global para tratamento de exceções
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

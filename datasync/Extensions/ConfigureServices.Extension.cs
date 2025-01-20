using Datasync.Core;
using DataSync.Infrastructure;
using MassTransit;
using Microsoft.OpenApi.Models;

namespace Datasync.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static void ConfigureProjectors(this IServiceCollection services)
        {
            services.AddScoped<IProjector, UserProjector>();
            services.AddScoped<IProjector, TowProjector>();
            services.AddScoped<IProjector, SupplierCompanyProjector>();
            services.AddScoped<IProjector, OrderProjector>();
            services.AddScoped<IProjector, TowDriverProjector>();
            services.AddSingleton<UserProjector>();
            services.AddSingleton<SupplierCompanyProjector>();
            services.AddSingleton<OrderProjector>();
            services.AddSingleton<TowDriverProjector>();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Datasync API", Version = "v1" });
            });
        }

        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumer<DatasyncController>();
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI")!), h =>
                    {
                        h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")!);
                        h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")!);
                    });

                    configurator.ConfigureEndpoints(context);
                });
            });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "User API", Version = "v1" });
            });
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User v1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}

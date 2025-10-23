using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MyProject.Application
{
    public static class DependencyInjection // 👈 Must be static
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(configuration => 
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddAutoMapper(configuration =>
            {
                configuration.AddMaps(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}

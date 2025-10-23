using Microsoft.Extensions.DependencyInjection;
using MyProject.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using MyProject.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Hangfire.SqlServer;
using Hangfire;

namespace MyProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    connectionString,
                    new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = false
                    }
                )
            );

            return services;
        }
    }
}

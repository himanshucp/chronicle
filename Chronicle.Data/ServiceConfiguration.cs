using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Data
{
    /// <summary>
    /// Extensions for configuring data services in Dependency Injection
    /// </summary>
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            // Register DapperContext as a singleton since it's lightweight
            services.AddSingleton<DapperContext>();

            // Register UnitOfWork as scoped (one per HTTP request)
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register your repositories here
            // Example:
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}

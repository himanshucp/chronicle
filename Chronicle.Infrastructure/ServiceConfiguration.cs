using Chronicle.Caching;
using Chronicle.Data;
using Chronicle.Lookups;
using Chronicle.Repositories;
using Chronicle.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chronicle.Lookups.LookupExtensions;

namespace Chronicle.Infrastructure
{
    /// <summary>
    /// Extension methods for configuring services in Dependency Injection
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registers all required services for the user management system
        /// </summary>
        public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
        {
            // Register data services
            services.AddSingleton<IDapperContext, DapperContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCache>();

            services.AddScoped<ILookup, Lookup>();
            services.AddScoped<ICacheClearEventHandler, LookupCacheClearHandler>();


            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDisciplineRepository, DisciplineRepository>();
            services.AddScoped<ISubDisciplineRepository, SubDisciplineRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();  

            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<ISubDisciplineService, SubDisciplineService>();  
            services.AddScoped<IContractService, ContractService>();    

            return services;
        }
    }
}

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
            services.AddScoped<ICompanyRoleRepository, CompanyRoleRepository>();
            services.AddScoped<ICompanyRoleRepository, CompanyRoleRepository>();
            services.AddScoped<IContractEmployeeRepository, ContractEmployeeRepository>();
            services.AddScoped<IContractEmployeeRoleRepository, ContractEmployeeRoleRepository>();

            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IWorkflowStepRepository, WorkflowStepRepository>();
            services.AddScoped<IWorkflowTransitionRepository, WorkflowTransitionRepository>();
            services.AddScoped<IWorkflowAssignmentRepository, WorkflowAssignmentRepository>();
            services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
            services.AddScoped<IWorkflowHistoryRepository, WorkflowHistoryRepository>();

            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<ISubDisciplineService, SubDisciplineService>();  
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<ICompanyRoleService, CompanyRoleService>();
            services.AddScoped<IContractEmployeeRoleService, ContractEmployeeRoleService>();

            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IWorkflowStepService, WorkflowStepService>();
            services.AddScoped<IWorkflowTransitionService, WorkflowTransitionService>();
            services.AddScoped<IWorkflowAssignmentService, WorkflowAssignmentService>();
            services.AddScoped<IWorkflowInstanceService, WorkflowInstanceService>();
            services.AddScoped<IWorkflowHistoryService, WorkflowHistoryService>();

            return services;
        }
    }
}

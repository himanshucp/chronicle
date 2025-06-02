using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IEmployeeModuleAccessRepository : IRepository<EmployeeModuleAccess,int>
    {
        Task<IEnumerable<EmployeeModuleAccess>> GetByContractEmployeeAsync(int contractEmployeeId, int tenantId);
        Task<IEnumerable<EmployeeModuleAccess>> GetByModuleAsync(int moduleId, int tenantId);
        Task<EmployeeModuleAccess> GetModuleAccessAsync(int contractEmployeeId, int moduleId, int tenantId);
        Task<bool> HasModuleAccessAsync(int contractEmployeeId, int moduleId, int tenantId);
        Task GrantModuleAccessAsync(int contractEmployeeId, int moduleId, int tenantId);
        Task RevokeModuleAccessAsync(int contractEmployeeId, int moduleId, int tenantId);
        Task<IEnumerable<AccessModule>> GetAccessibleModulesAsync(int contractEmployeeId, int tenantId);
    }
}

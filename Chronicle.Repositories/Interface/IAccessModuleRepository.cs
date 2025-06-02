using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IAccessModuleRepository : IRepository<AccessModule,int>
    {
        Task<AccessModule> GetByNameAsync(string moduleName, int tenantId);
        Task<IEnumerable<AccessModule>> GetActiveModulesAsync(int tenantId);
        Task<bool> IsModuleNameUniqueAsync(string moduleName, int tenantId, int? excludeModuleId = null);
        Task<IEnumerable<AccessModule>> GetModulesByTenantAccessAsync(int tenantId);
    }
}

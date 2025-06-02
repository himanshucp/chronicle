using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface ITenantModuleAccessRepository : IRepository<TenantModuleAccess,int>
    {
        Task<IEnumerable<TenantModuleAccess>> GetByTenantAsync(int tenantId);
        Task<TenantModuleAccess> GetTenantModuleAccessAsync(int tenantId, int moduleId);
        Task<bool> HasModuleAccessAsync(int tenantId, int moduleId);
        Task<IEnumerable<AccessModule>> GetAccessibleModulesAsync(int tenantId);
        Task GrantModuleAccessAsync(int tenantId, int moduleId, string accessLevel, int maxLicensedUsers);
        Task RevokeModuleAccessAsync(int tenantId, int moduleId);
        Task UpdateAccessLevelAsync(int tenantId, int moduleId, string accessLevel);
        Task UpdateMaxLicensedUsersAsync(int tenantId, int moduleId, int maxLicensedUsers);
    }

}

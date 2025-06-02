using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface ICompanyRoleRepository : IRepository<CompanyRole,int>
    {
        Task<CompanyRole> GetByNameAsync(string roleName, int tenantId);
        Task<IEnumerable<CompanyRole>> GetActiveCompanyRolesAsync(int tenantId);
        Task<bool> IsRoleNameUniqueAsync(string roleName, int tenantId, int? excludeRoleId = null);
    }
}

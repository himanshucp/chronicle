using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface  ITenantRepository : IRepository<Tenant, int>
    {
        Task<Tenant> GetByCodeAsync(string tenantCode);
   
        Task<bool> IsTenantNameUniqueAsync(string tenantName, int? excludeTenantId = null);
        Task<bool> IsTenantCodeUniqueAsync(string tenantCode, int? excludeTenantId = null);
    }
}

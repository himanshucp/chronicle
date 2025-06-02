using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    /// <summary>
    /// Role repository interface
    /// </summary>
    public interface IRoleRepository : IRepository<Role, int>
    {
        Task<Role> GetByNameAsync(string name, int tenantId);
        Task<IEnumerable<Role>> GetRolesByUserAsync(int userId, int tenantId);
    }
}

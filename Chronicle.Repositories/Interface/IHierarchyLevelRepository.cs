using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IHierarchyLevelRepository : IRepository<HierarchyLevel,int>
    {
        Task<HierarchyLevel> GetByNameAsync(string levelName, int tenantId);
        Task<IEnumerable<HierarchyLevel>> GetActiveHierarchyLevelsAsync(int tenantId);
        Task<bool> IsLevelNameUniqueAsync(string levelName, int tenantId, int? excludeLevelId = null);
    }
}

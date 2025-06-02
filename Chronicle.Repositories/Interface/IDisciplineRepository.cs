using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IDisciplineRepository : IRepository<Discipline, int>
    {
        Task<Discipline> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<Discipline>> GetAllAsync(int tenantId);
        Task<Discipline> GetByNameAsync(string name, int tenantId);
        Task<IEnumerable<Discipline>> GetActiveAsync(int tenantId);
        Task<PagedResult<Discipline>> GetPagedAsync(int page, int pageSize, string searchTerm, int tenantId);
    }
}

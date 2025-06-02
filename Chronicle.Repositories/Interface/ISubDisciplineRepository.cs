using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    /// <summary>
    /// Repository interface for SubDiscipline entity operations
    /// </summary>
    public interface ISubDisciplineRepository : IRepository<SubDiscipline, int>
    {
        Task<SubDiscipline> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<SubDiscipline>> GetAllAsync(int tenantId);
        Task<SubDiscipline> GetByNameAsync(string name, int tenantId);
        Task<IEnumerable<SubDiscipline>> GetActiveAsync(int tenantId);
        Task<PagedResult<SubDiscipline>> GetPagedAsync(int page, int pageSize, string searchTerm, int tenantId);
    }
}

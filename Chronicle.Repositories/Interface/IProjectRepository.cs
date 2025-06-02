using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IProjectRepository : IRepository<Project, int>
    {
        Task<Project> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<Project>> GetAllAsync(int tenantId);
        Task<Project> GetByProjectNumberAsync(string projectNumber, int tenantId);
        Task<IEnumerable<Project>> GetByOwnerCompanyAsync(int companyId, int tenantId);
        Task<IEnumerable<Project>> GetByProjectManagerAsync(int projectManagerId, int tenantId);
        Task<IEnumerable<Project>> GetByStatusAsync(string status, int tenantId);
        Task<IEnumerable<Project>> GetActiveProjectsAsync(int tenantId);
        Task<PagedResult<Project>> GetPagedAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<PagedResult<Project>> GetProjectsWithExpiringPermitsAsync(int daysThreshold, int page, int pageSize, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByTypeAsync(string projectType, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByLocationAsync(string location, int tenantId);
    }
}

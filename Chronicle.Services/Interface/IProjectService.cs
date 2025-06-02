using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IProjectService
    {
        Task<Project> GetProjectByIdAsync(int id, int tenantId);
        Task<IEnumerable<Project>> GetAllProjectsAsync(int tenantId);
        Task<int> CreateProjectAsync(Project project, int tenantId);
        Task<bool> UpdateProjectAsync(Project project, int tenantId);
        Task<bool> DeleteProjectAsync(int id, int tenantId);
        Task<Project> GetProjectByNumberAsync(string projectNumber, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByOwnerCompanyAsync(int companyId, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByManagerAsync(int projectManagerId, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status, int tenantId);
        Task<PagedResult<Project>> GetPagedProjectsAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<PagedResult<Project>> GetProjectsWithExpiringPermitsAsync(int daysThreshold, int page, int pageSize, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByTypeAsync(string projectType, int tenantId);
        Task<IEnumerable<Project>> GetProjectsByLocationAsync(string location, int tenantId);
        Task<bool> UpdateProjectStatusAsync(int id, string status, int tenantId);
        Task<bool> UpdateProjectDatesAsync(int id, DateTime? startDate, DateTime? endDate, int tenantId);
        Task<bool> UpdateProjectCostsAsync(int id, decimal? estimatedCost, decimal? actualCost, int tenantId);
        Task<bool> UpdateProjectPermitAsync(int id, string permitNumber, DateTime? issueDate, DateTime? expiryDate, int tenantId);
        Task<bool> ActivateProjectAsync(int id, int tenantId);
        Task<bool> DeactivateProjectAsync(int id, int tenantId);
    }

}

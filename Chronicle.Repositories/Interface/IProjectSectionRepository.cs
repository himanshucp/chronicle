using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface IProjectSectionRepository : IRepository<ProjectSection,int>
    {
        Task<IEnumerable<ProjectSection>> GetByProjectAsync(int projectId, int tenantId);
        Task<ProjectSection> GetByNameAsync(string sectionName, int projectId, int tenantId);
        Task<IEnumerable<ProjectSection>> GetActiveSectionsAsync(int tenantId);
        Task<bool> IsSectionNameUniqueInProjectAsync(string sectionName, int projectId, int tenantId, int? excludeSectionId = null);
    }
}

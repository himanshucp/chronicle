using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public  interface ISubDisciplineService
    {
        Task<SubDiscipline> GetSubDisciplineByIdAsync(int id, int tenantId);
        Task<IEnumerable<SubDiscipline>> GetAllSubDisciplinesAsync(int tenantId);
        Task<int> CreateSubDisciplineAsync(SubDiscipline subDiscipline, int tenantId);
        Task<bool> UpdateSubDisciplineAsync(SubDiscipline subDiscipline, int tenantId);
        Task<bool> DeleteSubDisciplineAsync(int id, int tenantId);
        Task<SubDiscipline> GetSubDisciplineByNameAsync(string name, int tenantId);
        Task<IEnumerable<SubDiscipline>> GetActiveSubDisciplinesAsync(int tenantId);
        Task<PagedResult<SubDiscipline>> GetPagedSubDisciplinesAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<bool> ActivateSubDisciplineAsync(int id, int tenantId);
        Task<bool> DeactivateSubDisciplineAsync(int id, int tenantId);
    }
}

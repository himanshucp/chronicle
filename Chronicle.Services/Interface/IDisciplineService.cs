using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface IDisciplineService
    {
        Task<Discipline> GetDisciplineByIdAsync(int id, int tenantId);
        Task<IEnumerable<Discipline>> GetAllDisciplinesAsync(int tenantId);
        Task<int> CreateDisciplineAsync(Discipline discipline, int tenantId);
        Task<bool> UpdateDisciplineAsync(Discipline discipline, int tenantId);
        Task<bool> DeleteDisciplineAsync(int id, int tenantId);
        Task<Discipline> GetDisciplineByNameAsync(string name, int tenantId);
        Task<IEnumerable<Discipline>> GetActiveDisciplinesAsync(int tenantId);
        Task<PagedResult<Discipline>> GetPagedDisciplinesAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<bool> ActivateDisciplineAsync(int id, int tenantId);
        Task<bool> DeactivateDisciplineAsync(int id, int tenantId);
    }
}

using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public interface ICompanyRepository : IRepository<Company, int>
    {
        Task<Company> GetByNameAsync(string companyName, int tenantId);

        Task<Company> GetByAbbrivationAsync(string abbrivation, int tenantId);

        Task<Company> GetByEmailAsync(string email, int tenantId);
        Task<IEnumerable<Company>> GetCompaniesByProjectAsync(int projectId, int tenantId);
        Task<Company> GetByIdAsync(int id, int tenantId);
        Task<IEnumerable<Company>> GetAllAsync(int tenantId);
        Task<IEnumerable<Company>> GetActiveCompaniesAsync(int tenantId);
        Task<PagedResult<Company>> GetCompaniesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId);
        Task<PagedResult<Company>> GetPagedAsync(int page, int pageSize, string searchTerm, int tenantId);
    }
}

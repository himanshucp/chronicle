using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public interface ICompanyService
    {
        Task<Company> GetCompanyByIdAsync(int id, int tenantId);
        Task<IEnumerable<Company>> GetAllCompaniesAsync(int tenantId);
        Task<int> CreateCompanyAsync(Company company, int tenantId);
        Task<bool> UpdateCompanyAsync(Company company, int tenantId);
        Task<bool> DeleteCompanyAsync(int id, int tenantId);
        Task<Company> GetCompanyByNameAsync(string name, int tenantId);
        Task<Company> GetCompanyByEmailAsync(string email, int tenantId);
        Task<IEnumerable<Company>> GetCompaniesByProjectAsync(int projectId, int tenantId);
        Task<PagedResult<Company>> GetPagedCompaniesAsync(int page, int pageSize, string searchTerm, int tenantId);
        Task<IEnumerable<Company>> GetActiveCompaniesAsync(int tenantId);
        Task<PagedResult<Company>> GetCompaniesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId);
        Task<bool> ActivateCompanyAsync(int id, int tenantId);
        Task<bool> DeactivateCompanyAsync(int id, int tenantId);
    }
}

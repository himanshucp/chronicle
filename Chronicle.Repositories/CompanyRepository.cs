using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Data.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Chronicle.Repositories
{
    public class CompanyRepository : DapperRepository<Company, int>, ICompanyRepository
    {
        public CompanyRepository(IUnitOfWork unitOfWork)
          : base(unitOfWork, "Companies", "CompanyID")
        {
        }

        public async Task<Company> GetByNameAsync(string companyName, int tenantId)
        {
            const string sql = "SELECT * FROM Companies WHERE Name = @Name AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Company>(
                sql,
                new { Name = companyName, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Company> GetByAbbrivationAsync(string abbrivation, int tenantId)
        {
            const string sql = "SELECT * FROM Companies WHERE Abbrivation = @Abbrivation AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Company>(
                sql,
                new { Abbrivation = abbrivation, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Company> GetByEmailAsync(string email, int tenantId)
        {
            const string sql = "SELECT * FROM Companies WHERE Email = @Email AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Company>(
                sql,
                new { Email = email, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Company>> GetCompaniesByProjectAsync(int projectId, int tenantId)
        {
            const string sql = @"
                SELECT c.* FROM Companies c
                INNER JOIN ProjectCompanies pc ON c.CompanyID = pc.CompanyID
                INNER JOIN Projects p ON pc.ProjectID = p.ProjectID
                WHERE pc.ProjectID = @ProjectID AND p.TenantID = @TenantID";

            return await _unitOfWork.Connection.QueryAsync<Company>(
                sql,
                new { ProjectID = projectId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Companies WHERE IsActive = 1 AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Company>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Company>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Companies WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Company>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Company>> GetCompaniesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId)
        {
            var currentDate = DateTime.UtcNow;
            var expiryDate = currentDate.AddDays(daysThreshold);

            string whereClause = "LicenseExpiryDate IS NOT NULL AND LicenseExpiryDate <= @ExpiryDate AND IsActive = 1 AND TenantID = @TenantID";
            var parameters = new { ExpiryDate = expiryDate, TenantID = tenantId };

            return await _unitOfWork.Connection.QueryPagedAsync<Company>(
                "Companies",
                "Name",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(Company company)
        {
            const string sql = @"
                INSERT INTO Companies (
                    TenantID, Name, Abbrivation, Location, Address, Email, 
                    ContactPerson, Phone, Fax, TaxNumber, LicenseNumber, 
                    LicenseExpiryDate, InsuranceDetails, CreatedDate, LastModifiedDate, IsActive,WebSite)
                VALUES (
                    @TenantID, @Name, @Abbrivation, @Location, @Address, @Email, 
                    @ContactPerson, @Phone, @Fax, @TaxNumber, @LicenseNumber, 
                    @LicenseExpiryDate, @InsuranceDetails, @CreatedDate, @LastModifiedDate, @IsActive,@WebSite);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (company.CreatedDate == null)
            {
                company.CreatedDate = DateTime.UtcNow;
                company.LastModifiedDate = DateTime.UtcNow;
            }

            // Set IsActive to true by default if not specified
            if (company.IsActive == null)
            {
                company.IsActive = true;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
              sql,
              company,
              _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Company company)
        {
            const string sql = @"
                UPDATE Companies
                SET TenantID = @TenantID,
                    Name = @Name,
                    Abbrivation = @Abbrivation,
                    Location = @Location,
                    Address = @Address,
                    Email = @Email,
                    ContactPerson = @ContactPerson,
                    Phone = @Phone,
                    Fax = @Fax,
                    TaxNumber = @TaxNumber,
                    LicenseNumber = @LicenseNumber,
                    LicenseExpiryDate = @LicenseExpiryDate,
                    InsuranceDetails = @InsuranceDetails,
                    LastModifiedDate = @LastModifiedDate,
                    IsActive = @IsActive,
                    WebSite = @WebSite
                WHERE CompanyID = @CompanyID";

            // Set modification date
            company.LastModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                company,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<PagedResult<Company>> GetPagedAsync(int page, int pageSize, string searchTerm = null, int tenantId = 0)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    TenantID = @TenantID AND (
                    Name LIKE @SearchTerm OR 
                    Abbrivation LIKE @SearchTerm OR
                    Location LIKE @SearchTerm OR
                    Address LIKE @SearchTerm OR
                    Email LIKE @SearchTerm OR
                    ContactPerson LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Company>(
                "Companies",
                "Name",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            // This maintains backward compatibility with base class
            const string sql = "SELECT * FROM Companies WHERE CompanyID = @CompanyID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Company>(
                sql,
                new { CompanyID = id },
                _unitOfWork.Transaction);
        }

        public async Task<Company> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM Companies WHERE CompanyID = @CompanyID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Company>(
                sql,
                new { CompanyID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }
    }

}

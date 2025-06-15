using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Company> GetCompanyByIdAsync(int id, int tenantId)
        {
            return await _companyRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(int tenantId)
        {
            return await _companyRepository.GetAllAsync(tenantId);
        }

        public async Task<int> CreateCompanyAsync(Company company, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure company belongs to the right tenant
                company.TenantID = tenantId;

                // Validate company data
                if (string.IsNullOrEmpty(company.Name))
                {
                    throw new ArgumentException("Company name is required");
                }

                // Check if company with same name already exists in this tenant
                var existingCompany = await _companyRepository.GetByNameAsync(company.Name, tenantId);
                if (existingCompany != null && existingCompany.CompanyID != company.CompanyID)
                {
                    throw new InvalidOperationException($"A company with name '{company.Name}' already exists");
                }

                var existingAbbrivation =await _companyRepository.GetByAbbrivationAsync(company.Abbrivation, tenantId);
                if (existingCompany != null && existingCompany.CompanyID != company.CompanyID)
                {
                    throw new InvalidOperationException($"A Abbrivation with '{company.Abbrivation}' for  '{company.Name}' already exists");
                }

                // Check if company with same email already exists in this tenant
                if (!string.IsNullOrEmpty(company.Email))
                {
                    var existingEmail = await _companyRepository.GetByEmailAsync(company.Email, tenantId);
                    if (existingEmail != null && existingEmail.CompanyID != company.CompanyID)
                    {
                        throw new InvalidOperationException($"A company with email '{company.Email}' already exists");
                    }
                }

                // Set audit fields
                company.CreatedDate = DateTime.UtcNow;
                company.LastModifiedDate = DateTime.UtcNow;
                //company.IsActive = true;

                int id = await _companyRepository.InsertAsync(company);
                await _unitOfWork.CommitAsync();
                return id;
            }
            catch (Exception ex) 
            {
                await _unitOfWork.RollbackAsync();
                    string errorMessage = ex.Message;
              
                throw;
            }
        }

        public async Task<bool> UpdateCompanyAsync(Company company, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate company data
                if (string.IsNullOrEmpty(company.Name))
                {
                    throw new ArgumentException("Company name is required");
                }

                // Get existing company and verify it belongs to this tenant
                var existingCompany = await _companyRepository.GetByIdAsync(company.CompanyID, tenantId);
                if (existingCompany == null)
                {
                    throw new InvalidOperationException($"Company with ID {company.CompanyID} not found");
                }

            
                // Ensure tenant ID cannot be changed
                company.TenantID = tenantId;

                // Check if another company with same name exists in this tenant
                var nameCheck = await _companyRepository.GetByNameAsync(company.Name, tenantId);
                if (nameCheck != null && nameCheck.CompanyID != company.CompanyID)
                {
                    throw new InvalidOperationException($"Another company with name '{company.Name}' already exists");
                }

                var abbrivationCheck = await _companyRepository.GetByAbbrivationAsync(company.Abbrivation, tenantId);
                if (abbrivationCheck != null && abbrivationCheck.CompanyID != company.CompanyID)
                {
                    throw new InvalidOperationException($"A Abbrivation with '{company.Abbrivation}' for  '{company.Name}' already exists");
                }


                // Check if another company with same email exists in this tenant
                if (!string.IsNullOrEmpty(company.Email))
                {
                    var emailCheck = await _companyRepository.GetByEmailAsync(company.Email, tenantId);
                    if (emailCheck != null && emailCheck.CompanyID != company.CompanyID)
                    {
                        throw new InvalidOperationException($"Another company with email '{company.Email}' already exists");
                    }
                }

                // Set audit fields
                company.LastModifiedDate = DateTime.UtcNow;

                bool result = await _companyRepository.UpdateAsync(company);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteCompanyAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify company belongs to this tenant
                var company = await _companyRepository.GetByIdAsync(id, tenantId);
                if (company == null)
                {
                    throw new InvalidOperationException($"Company with ID {id} not found");
                }

                // Perform soft delete by changing the IsActive flag
                company.IsActive = false;
                company.LastModifiedDate = DateTime.UtcNow;

                bool result = await _companyRepository.UpdateAsync(company);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Company> GetCompanyByNameAsync(string name, int tenantId)
        {
            return await _companyRepository.GetByNameAsync(name, tenantId);
        }

        public async Task<Company> GetByAbbrivationAsync(string abbrivation, int tenantId)
        {
            return await _companyRepository.GetByAbbrivationAsync(abbrivation, tenantId);
        }

        public async Task<Company> GetCompanyByEmailAsync(string email, int tenantId)
        {
            return await _companyRepository.GetByEmailAsync(email, tenantId);
        }

        public async Task<IEnumerable<Company>> GetCompaniesByProjectAsync(int projectId, int tenantId)
        {
            return await _companyRepository.GetCompaniesByProjectAsync(projectId, tenantId);
        }

        public async Task<PagedResult<Company>> GetPagedCompaniesAsync(int page, int pageSize, string searchTerm, int tenantId)
        {
            return await _companyRepository.GetPagedAsync(page, pageSize, searchTerm, tenantId);
        }



        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync(int tenantId)
        {
            return await _companyRepository.GetActiveCompaniesAsync(tenantId);
        }

        public async Task<PagedResult<Company>> GetCompaniesWithExpiringLicensesAsync(int daysThreshold, int page, int pageSize, int tenantId)
        {
            return await _companyRepository.GetCompaniesWithExpiringLicensesAsync(daysThreshold, page, pageSize, tenantId);
        }

        public async Task<bool> ActivateCompanyAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify company belongs to this tenant
                var company = await _companyRepository.GetByIdAsync(id, tenantId);
                if (company == null)
                {
                    throw new InvalidOperationException($"Company with ID {id} not found");
                }

                company.IsActive = true;
                company.LastModifiedDate = DateTime.UtcNow;

                bool result = await _companyRepository.UpdateAsync(company);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeactivateCompanyAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify company belongs to this tenant
                var company = await _companyRepository.GetByIdAsync(id, tenantId);
                if (company == null)
                {
                    throw new InvalidOperationException($"Company with ID {id} not found");
                }

                company.IsActive = false;
                company.LastModifiedDate = DateTime.UtcNow;

                bool result = await _companyRepository.UpdateAsync(company);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }

}

using Chronicle.Data;
using Chronicle.Entities;
using Chronicle.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Services
{

    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(
            IProjectRepository projectRepository,
            ICompanyRepository companyRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Project> GetProjectByIdAsync(int id, int tenantId)
        {
            return await _projectRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(int tenantId)
        {
            return await _projectRepository.GetAllAsync(tenantId);
        }

        public async Task<int> CreateProjectAsync(Project project, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ensure project belongs to the right tenant
                project.TenantID = tenantId;

                // Validate project data
                if (string.IsNullOrEmpty(project.ProjectName))
                {
                    throw new ArgumentException("Project name is required");
                }

                // Generate a project number if not provided
                if (string.IsNullOrEmpty(project.ProjectNumber))
                {
                    project.ProjectNumber = await GenerateProjectNumberAsync(tenantId);
                }
                else
                {
                    // Check if project with same number already exists in this tenant
                    var existingProject = await _projectRepository.GetByProjectNumberAsync(project.ProjectNumber, tenantId);
                    if (existingProject != null)
                    {
                        throw new InvalidOperationException($"A project with number '{project.ProjectNumber}' already exists");
                    }
                }

                // Validate owner company exists and belongs to this tenant
                if (project.OwnerCompanyID > 0)
                {
                    var company = await _companyRepository.GetByIdAsync(project.OwnerCompanyID, tenantId);
                    if (company == null)
                    {
                        throw new InvalidOperationException($"Owner company with ID {project.OwnerCompanyID} not found or does not belong to this tenant");
                    }
                }

                // Validate project manager exists and belongs to this tenant
                if (project.ProjectManagerID.HasValue && project.ProjectManagerID.Value > 0)
                {
                    var manager = await _employeeRepository.GetByIdAsync(project.ProjectManagerID.Value, tenantId);
                    if (manager == null)
                    {
                        throw new InvalidOperationException($"Project manager with ID {project.ProjectManagerID} not found or does not belong to this tenant");
                    }
                }

                // Set default project status if not provided
                if (string.IsNullOrEmpty(project.ProjectStatus))
                {
                    project.ProjectStatus = "New";
                }

                // Set audit fields
                project.CreatedDate = DateTime.UtcNow;
                project.ModifiedDate = DateTime.UtcNow;
                project.IsActive = true;

                int id = await _projectRepository.InsertAsync(project);
                await _unitOfWork.CommitAsync();
                return id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateProjectAsync(Project project, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate project data
                if (string.IsNullOrEmpty(project.ProjectName))
                {
                    throw new ArgumentException("Project name is required");
                }

                // Get existing project and verify it belongs to this tenant
                var existingProject = await _projectRepository.GetByIdAsync(project.ProjectID, tenantId);
                if (existingProject == null)
                {
                    throw new InvalidOperationException($"Project with ID {project.ProjectID} not found");
                }

                // Ensure tenant ID cannot be changed
                project.TenantID = tenantId;

                // Check if project number is being changed, and if so, check if it already exists
                if (project.ProjectNumber != existingProject.ProjectNumber)
                {
                    var projectWithNumber = await _projectRepository.GetByProjectNumberAsync(project.ProjectNumber, tenantId);
                    if (projectWithNumber != null && projectWithNumber.ProjectID != project.ProjectID)
                    {
                        throw new InvalidOperationException($"Another project with number '{project.ProjectNumber}' already exists");
                    }
                }

                // Validate owner company exists and belongs to this tenant
                if (project.OwnerCompanyID > 0)
                {
                    var company = await _companyRepository.GetByIdAsync(project.OwnerCompanyID, tenantId);
                    if (company == null)
                    {
                        throw new InvalidOperationException($"Owner company with ID {project.OwnerCompanyID} not found or does not belong to this tenant");
                    }
                }

                // Validate project manager exists and belongs to this tenant
                if (project.ProjectManagerID.HasValue && project.ProjectManagerID.Value > 0)
                {
                    var manager = await _employeeRepository.GetByIdAsync(project.ProjectManagerID.Value, tenantId);
                    if (manager == null)
                    {
                        throw new InvalidOperationException($"Project manager with ID {project.ProjectManagerID} not found or does not belong to this tenant");
                    }
                }

                // Set audit fields
                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteProjectAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                // Perform soft delete by setting IsActive to false
                project.IsActive = false;
                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<Project> GetProjectByNumberAsync(string projectNumber, int tenantId)
        {
            return await _projectRepository.GetByProjectNumberAsync(projectNumber, tenantId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByOwnerCompanyAsync(int companyId, int tenantId)
        {
            // Verify company belongs to this tenant
            var company = await _companyRepository.GetByIdAsync(companyId, tenantId);
            if (company == null)
            {
                throw new InvalidOperationException($"Company with ID {companyId} not found or does not belong to this tenant");
            }

            return await _projectRepository.GetByOwnerCompanyAsync(companyId, tenantId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByManagerAsync(int projectManagerId, int tenantId)
        {
            // Verify manager belongs to this tenant
            var manager = await _employeeRepository.GetByIdAsync(projectManagerId, tenantId);
            if (manager == null)
            {
                throw new InvalidOperationException($"Employee with ID {projectManagerId} not found or does not belong to this tenant");
            }

            return await _projectRepository.GetByProjectManagerAsync(projectManagerId, tenantId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status, int tenantId)
        {
            return await _projectRepository.GetByStatusAsync(status, tenantId);
        }

        public async Task<PagedResult<Project>> GetPagedProjectsAsync(int page, int pageSize, string searchTerm, int tenantId)
        {
            return await _projectRepository.GetPagedAsync(page, pageSize, searchTerm, tenantId);
        }

        public async Task<PagedResult<Project>> GetProjectsWithExpiringPermitsAsync(int daysThreshold, int page, int pageSize, int tenantId)
        {
            return await _projectRepository.GetProjectsWithExpiringPermitsAsync(daysThreshold, page, pageSize, tenantId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            return await _projectRepository.GetProjectsByDateRangeAsync(startDate, endDate, tenantId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByTypeAsync(string projectType, int tenantId)
        {
            return await _projectRepository.GetProjectsByTypeAsync(projectType, tenantId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByLocationAsync(string location, int tenantId)
        {
            return await _projectRepository.GetProjectsByLocationAsync(location, tenantId);
        }

        public async Task<bool> UpdateProjectStatusAsync(int id, string status, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                // Validate status
                if (string.IsNullOrEmpty(status))
                {
                    throw new ArgumentException("Project status cannot be empty");
                }

                project.ProjectStatus = status;
                project.ModifiedDate = DateTime.UtcNow;

                // If status is "Completed", set actual end date if not already set
                if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase) && !project.ActualEndDate.HasValue)
                {
                    project.ActualEndDate = DateTime.UtcNow;
                }

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateProjectDatesAsync(int id, DateTime? startDate, DateTime? endDate, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                // Validate dates
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    throw new ArgumentException("Start date cannot be after end date");
                }

                // Update only the fields that are provided
                if (startDate.HasValue)
                {
                    project.ActualStartDate = startDate;
                }

                if (endDate.HasValue)
                {
                    project.ActualEndDate = endDate;
                }

                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateProjectCostsAsync(int id, decimal? estimatedCost, decimal? actualCost, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                // Validate costs
                if (estimatedCost.HasValue && estimatedCost < 0)
                {
                    throw new ArgumentException("Estimated cost cannot be negative");
                }

                if (actualCost.HasValue && actualCost < 0)
                {
                    throw new ArgumentException("Actual cost cannot be negative");
                }

                // Update only the fields that are provided
                if (estimatedCost.HasValue)
                {
                    project.EstimatedCost = estimatedCost;
                }

                if (actualCost.HasValue)
                {
                    project.ActualCost = actualCost;
                }

                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateProjectPermitAsync(int id, string permitNumber, DateTime? issueDate, DateTime? expiryDate, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                // Validate permit dates
                if (issueDate.HasValue && expiryDate.HasValue && issueDate > expiryDate)
                {
                    throw new ArgumentException("Permit issue date cannot be after expiry date");
                }

                // Update permit information
                project.PermitNumber = permitNumber;
                project.PermitIssueDate = issueDate;
                project.PermitExpiryDate = expiryDate;
                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ActivateProjectAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                project.IsActive = true;
                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeactivateProjectAsync(int id, int tenantId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Verify project belongs to this tenant
                var project = await _projectRepository.GetByIdAsync(id, tenantId);
                if (project == null)
                {
                    throw new InvalidOperationException($"Project with ID {id} not found");
                }

                project.IsActive = false;
                project.ModifiedDate = DateTime.UtcNow;

                bool result = await _projectRepository.UpdateAsync(project);

                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // Helper method to generate a unique project number
        private async Task<string> GenerateProjectNumberAsync(int tenantId)
        {
            // Get current year
            int year = DateTime.UtcNow.Year;

            // Get current count of projects for this year
            const string sql = @"
                SELECT COUNT(*) 
                FROM Projects 
                WHERE TenantID = @TenantID 
                AND ProjectNumber LIKE @YearPrefix";

            int count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                sql,
                new { TenantID = tenantId, YearPrefix = $"P-{year}%" },
                _unitOfWork.Transaction);

            // Generate new project number in format P-YYYY-XXXX where XXXX is sequential
            string projectNumber = $"P-{year}-{(count + 1):D4}";

            // Check if this project number already exists (just to be safe)
            var existingProject = await _projectRepository.GetByProjectNumberAsync(projectNumber, tenantId);
            if (existingProject != null)
            {
                // In the unlikely event of a collision, try the next number
                projectNumber = $"P-{year}-{(count + 2):D4}";
            }

            return projectNumber;
        }
    }

}

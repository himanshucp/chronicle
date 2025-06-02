using Chronicle.Data;
using Chronicle.Data.Extensions;
using Chronicle.Entities;
using Chronicle.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public class ProjectRepository : DapperRepository<Project, int>, IProjectRepository
    {
        public ProjectRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Projects", "ProjectID")
        {
        }

        public async Task<Project> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE ProjectID = @ProjectID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Project>(
                sql,
                new { ProjectID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Project> GetByProjectNumberAsync(string projectNumber, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE ProjectNumber = @ProjectNumber AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Project>(
                sql,
                new { ProjectNumber = projectNumber, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetByOwnerCompanyAsync(int companyId, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE OwnerCompanyID = @OwnerCompanyID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { OwnerCompanyID = companyId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetByProjectManagerAsync(int projectManagerId, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE ProjectManagerID = @ProjectManagerID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { ProjectManagerID = projectManagerId, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetByStatusAsync(string status, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE ProjectStatus = @ProjectStatus AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { ProjectStatus = status, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE IsActive = 1 AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Project>> GetPagedAsync(int page, int pageSize, string searchTerm = null, int tenantId = 0)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    TenantID = @TenantID AND (
                    ProjectName LIKE @SearchTerm OR 
                    ProjectNumber LIKE @SearchTerm OR
                    Description LIKE @SearchTerm OR
                    ProjectType LIKE @SearchTerm OR
                    Location LIKE @SearchTerm OR
                    Address LIKE @SearchTerm OR
                    ProjectStatus LIKE @SearchTerm OR
                    PermitNumber LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Project>(
                "Projects",
                "ProjectName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Project>> GetProjectsWithExpiringPermitsAsync(int daysThreshold, int page, int pageSize, int tenantId)
        {
            var currentDate = DateTime.UtcNow;
            var expiryDate = currentDate.AddDays(daysThreshold);

            string whereClause = @"
                PermitExpiryDate IS NOT NULL AND 
                PermitExpiryDate <= @ExpiryDate AND 
                IsActive = 1 AND 
                TenantID = @TenantID";

            var parameters = new { ExpiryDate = expiryDate, TenantID = tenantId };

            return await _unitOfWork.Connection.QueryPagedAsync<Project>(
                "Projects",
                "ProjectName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetProjectsByDateRangeAsync(DateTime startDate, DateTime endDate, int tenantId)
        {
            const string sql = @"
                SELECT * FROM Projects 
                WHERE TenantID = @TenantID
                AND (
                    (EstimatedStartDate IS NOT NULL AND EstimatedStartDate BETWEEN @StartDate AND @EndDate)
                    OR (ActualStartDate IS NOT NULL AND ActualStartDate BETWEEN @StartDate AND @EndDate)
                    OR (EstimatedEndDate IS NOT NULL AND EstimatedEndDate BETWEEN @StartDate AND @EndDate)
                    OR (ActualEndDate IS NOT NULL AND ActualEndDate BETWEEN @StartDate AND @EndDate)
                )";

            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { StartDate = startDate, EndDate = endDate, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetProjectsByTypeAsync(string projectType, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE ProjectType = @ProjectType AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { ProjectType = projectType, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Project>> GetProjectsByLocationAsync(string location, int tenantId)
        {
            const string sql = "SELECT * FROM Projects WHERE Location = @Location AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Project>(
                sql,
                new { Location = location, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(Project project)
        {
            const string sql = @"
                INSERT INTO Projects (
                    TenantID, ProjectName, ProjectNumber, Description, ProjectType, Location, 
                    Address, OwnerCompanyID, EstimatedCost, ActualCost, EstimatedStartDate, 
                    ActualStartDate, EstimatedEndDate, ActualEndDate, ProjectStatus, 
                    ProjectManagerID, PermitNumber, PermitIssueDate, PermitExpiryDate, 
                    CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @TenantID, @ProjectName, @ProjectNumber, @Description, @ProjectType, @Location, 
                    @Address, @OwnerCompanyID, @EstimatedCost, @ActualCost, @EstimatedStartDate, 
                    @ActualStartDate, @EstimatedEndDate, @ActualEndDate, @ProjectStatus, 
                    @ProjectManagerID, @PermitNumber, @PermitIssueDate, @PermitExpiryDate, 
                    @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (project.CreatedDate == null)
            {
                project.CreatedDate = DateTime.UtcNow;
                project.ModifiedDate = DateTime.UtcNow;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                project,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Project project)
        {
            const string sql = @"
                UPDATE Projects
                SET TenantID = @TenantID,
                    ProjectName = @ProjectName,
                    ProjectNumber = @ProjectNumber,
                    Description = @Description,
                    ProjectType = @ProjectType,
                    Location = @Location,
                    Address = @Address,
                    OwnerCompanyID = @OwnerCompanyID,
                    EstimatedCost = @EstimatedCost,
                    ActualCost = @ActualCost,
                    EstimatedStartDate = @EstimatedStartDate,
                    ActualStartDate = @ActualStartDate,
                    EstimatedEndDate = @EstimatedEndDate,
                    ActualEndDate = @ActualEndDate,
                    ProjectStatus = @ProjectStatus,
                    ProjectManagerID = @ProjectManagerID,
                    PermitNumber = @PermitNumber,
                    PermitIssueDate = @PermitIssueDate,
                    PermitExpiryDate = @PermitExpiryDate,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE ProjectID = @ProjectID";

            // Set modification date
            project.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                project,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Implementing soft delete
            const string sql = @"
                UPDATE Projects 
                SET IsActive = 0, 
                    ModifiedDate = @ModifiedDate 
                WHERE ProjectID = @ProjectID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { ProjectID = id, ModifiedDate = DateTime.UtcNow },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}

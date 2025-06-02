using Chronicle.Data.Extensions;
using Chronicle.Data;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    public class DisciplineRepository : DapperRepository<Discipline, int>, IDisciplineRepository
    {
        public DisciplineRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork, "Disciplines", "DisciplineID")
        {
        }

        public async Task<Discipline> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM Disciplines WHERE DisciplineID = @DisciplineID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Discipline>(
                sql,
                new { DisciplineID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Discipline>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Disciplines WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Discipline>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<Discipline> GetByNameAsync(string name, int tenantId)
        {
            const string sql = "SELECT * FROM Disciplines WHERE DisciplineName = @DisciplineName AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Discipline>(
                sql,
                new { DisciplineName = name, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Discipline>> GetActiveAsync(int tenantId)
        {
            const string sql = "SELECT * FROM Disciplines WHERE IsActive = 1 AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<Discipline>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        public async Task<PagedResult<Discipline>> GetPagedAsync(int page, int pageSize, string searchTerm = null, int tenantId = 0)
        {
            string whereClause = "TenantID = @TenantID";
            object parameters = new { TenantID = tenantId };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause = @"
                    TenantID = @TenantID AND (
                    DisciplineName LIKE @SearchTerm OR 
                    Description LIKE @SearchTerm)";

                parameters = new { TenantID = tenantId, SearchTerm = $"%{searchTerm}%" };
            }

            return await _unitOfWork.Connection.QueryPagedAsync<Discipline>(
                "Disciplines",
                "DisciplineName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public override async Task<int> InsertAsync(Discipline discipline)
        {
            const string sql = @"
                INSERT INTO Disciplines (
                    TenantID, DisciplineName, Description, CreatedDate, ModifiedDate, IsActive)
                VALUES (
                    @TenantID, @DisciplineName, @Description, @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // Set creation date if not set
            if (discipline.CreatedDate == null)
            {
                discipline.CreatedDate = DateTime.UtcNow;
                discipline.ModifiedDate = DateTime.UtcNow;
            }

            // Set IsActive to true by default if not specified
            if (discipline.IsActive == null)
            {
                discipline.IsActive = true;
            }

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                sql,
                discipline,
                _unitOfWork.Transaction);
        }

        public override async Task<bool> UpdateAsync(Discipline discipline)
        {
            const string sql = @"
                UPDATE Disciplines
                SET TenantID = @TenantID,
                    DisciplineName = @DisciplineName,
                    Description = @Description,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE DisciplineID = @DisciplineID";

            // Set modification date
            discipline.ModifiedDate = DateTime.UtcNow;

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                discipline,
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Implementing soft delete
            const string sql = @"
                UPDATE Disciplines 
                SET IsActive = 0, 
                    ModifiedDate = @ModifiedDate 
                WHERE DisciplineID = @DisciplineID";

            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { DisciplineID = id, ModifiedDate = DateTime.UtcNow },
                _unitOfWork.Transaction);

            return rowsAffected > 0;
        }
    }
}

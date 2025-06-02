using Chronicle.Data;
using Chronicle.Data.Extensions;
using Chronicle.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    /// <summary>
    /// Repository implementation for SubDiscipline entity operations using Dapper
    /// </summary>
    public class SubDisciplineRepository : DapperRepository<SubDiscipline, int>, ISubDisciplineRepository
    {
        public SubDisciplineRepository(IUnitOfWork unitOfWork)
              : base(unitOfWork, "SubDiscipline", "SubDisciplineID")
        {
        }

        /// <inheritdoc/>
        public async Task<SubDiscipline> GetByIdAsync(int id, int tenantId)
        {
            const string sql = "SELECT * FROM SubDisciplines WHERE SubDisciplineID = @SubDisciplineID AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<SubDiscipline>(
                sql,
                new { SubDisciplineID = id, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubDiscipline>> GetAllAsync(int tenantId)
        {
            const string sql = "SELECT * FROM SubDisciplines WHERE TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<SubDiscipline>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);

            return await _unitOfWork.Connection.QueryAsync<SubDiscipline>(
                sql,
                transaction: _unitOfWork.Transaction);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SubDiscipline>> GetActiveAsync(int tenantId)
        {
            const string sql = "SELECT * FROM SubDisciplines WHERE IsActive = 1 AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryAsync<SubDiscipline>(
                sql,
                new { TenantID = tenantId },
                _unitOfWork.Transaction);
        }

        /// <inheritdoc/>
      
        //public async Task<IEnumerable<SubDiscipline>> GetByDisciplineAsync(int disciplineId)
        //{
        //    var query = @"
        //        SELECT SubDisciplineID, SubDisciplineName, Description, TenantID, 
        //               DisciplineID, CreatedDate, ModifiedDate, IsActive
        //        FROM SubDisciplines
        //        WHERE DisciplineID = @DisciplineId";

        //    return await _unitOfWork.Connection.QueryAsync<SubDiscipline>(
        //        query,
        //        new { DisciplineId = disciplineId },
        //        _unitOfWork.Transaction);
        //}

        /// <inheritdoc/>
        public override async Task<int> InsertAsync(SubDiscipline entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Ensure dates are set
            entity.CreatedDate ??= DateTime.UtcNow;
            entity.ModifiedDate ??= entity.CreatedDate;
            entity.IsActive ??= true;

            var query = @"
                INSERT INTO SubDisciplines (SubDisciplineName, Description, TenantID, 
                                          DisciplineID, CreatedDate, ModifiedDate, IsActive)
                VALUES (@SubDisciplineName, @Description, @TenantID, 
                        @DisciplineID, @CreatedDate, @ModifiedDate, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _unitOfWork.Connection.QuerySingleAsync<int>(
                query,
                entity,
                _unitOfWork.Transaction);
        }

        /// <inheritdoc/>
        public override async Task<bool> UpdateAsync(SubDiscipline entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.ModifiedDate = DateTime.UtcNow;

            var query = @"
                UPDATE SubDisciplines
                SET SubDisciplineName = @SubDisciplineName,
                    Description = @Description,
                    TenantID = @TenantID,
                    DisciplineID = @DisciplineID,
                    ModifiedDate = @ModifiedDate,
                    IsActive = @IsActive
                WHERE SubDisciplineID = @SubDisciplineID";

            var affectedRows = await _unitOfWork.Connection.ExecuteAsync(
                query,
                entity,
                _unitOfWork.Transaction);

            return affectedRows > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            var query = @"
                DELETE FROM SubDisciplines
                WHERE SubDisciplineID = @Id";

            var affectedRows = await _unitOfWork.Connection.ExecuteAsync(
                query,
                new { Id = id },
                _unitOfWork.Transaction);

            return affectedRows > 0;
        }


        public async Task<PagedResult<SubDiscipline>> GetPagedAsync(int page, int pageSize, string searchTerm = null, int tenantId = 0)
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

            return await _unitOfWork.Connection.QueryPagedAsync<SubDiscipline>(
                "SubDisciplines",
                "SubDisciplineName",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }

        public async Task<SubDiscipline> GetByNameAsync(string name, int tenantId)
        {
            const string sql = "SELECT * FROM SubDisciplines WHERE SubDisciplineName = @SubDisciplineName AND TenantID = @TenantID";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<SubDiscipline>(
                sql,
                new { SubDisciplineName = name, TenantID = tenantId },
                _unitOfWork.Transaction);
        }

      


        /// <inheritdoc/>



    }
}

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
    /// Base repository implementation using Dapper
    /// </summary>
    public abstract class DapperRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly string _tableName;
        protected readonly string _idColumn;

        protected DapperRepository(IUnitOfWork unitOfWork, string tableName, string idColumn)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            _idColumn = idColumn ?? throw new ArgumentNullException(nameof(idColumn));
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id, int tenantId)
        {
            string sql = $"SELECT * FROM {_tableName} WHERE {_idColumn} = @Id AND TenantID = @TenantId";
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<TEntity>(
                sql,
                new { Id = id, TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(int tenantId)
        {
            string sql = $"SELECT * FROM {_tableName} WHERE TenantID = @TenantId";
            return await _unitOfWork.Connection.QueryAsync<TEntity>(
                sql,
                new { TenantId = tenantId },
                _unitOfWork.Transaction);
        }

        public abstract Task<int> InsertAsync(TEntity entity);

        public abstract Task<bool> UpdateAsync(TEntity entity);

        public virtual async Task<bool> DeleteAsync(TKey id, int tenantId)
        {
            string sql = $"DELETE FROM {_tableName} WHERE {_idColumn} = @Id AND TenantID = @TenantId";
            int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(
                sql,
                new { Id = id, TenantId = tenantId },
                _unitOfWork.Transaction);
            return rowsAffected > 0;
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null)
        {
            string whereClause = "TenantID = @TenantId";
            var parameters = new { TenantId = tenantId };

            return await _unitOfWork.Connection.QueryPagedAsync<TEntity>(
                _tableName,
                $"{_idColumn} DESC",
                page,
                pageSize,
                whereClause,
                parameters,
                _unitOfWork.Transaction);
        }
    }
}

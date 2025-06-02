using Chronicle.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Repositories
{
    /// <summary>
    /// Generic repository interface
    /// </summary>
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(TKey id, int tenantId);
        Task<IEnumerable<TEntity>> GetAllAsync(int tenantId);
        Task<int> InsertAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TKey id, int tenantId);
        Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, int tenantId, string searchTerm = null);
    }
}

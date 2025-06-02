using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronicle.Data
{
    /// <summary>
    /// Interface for transaction management
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void Commit();
        Task CommitAsync();

        void Rollback();
        Task RollbackAsync();
    }
}

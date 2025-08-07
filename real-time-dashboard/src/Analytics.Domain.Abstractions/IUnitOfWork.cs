using System;
using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Unit of Work pattern interface for transaction management
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begin a new transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        Task RollbackAsync();

        /// <summary>
        /// Save changes to the database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Check if there are unsaved changes
        /// </summary>
        bool HasUnsavedChanges { get; }
    }
}


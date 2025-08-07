using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get entity by ID
        /// </summary>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// Get all entities
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find entities by predicate
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add new entity
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Update existing entity
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Check if entity exists
        /// </summary>
        Task<bool> ExistsAsync(object id);
    }
}


// Copyright (c) DigiOutsource. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Dashboards entity operations
    /// </summary>
    public interface IDashboardsRepository
    {
        /// <summary>
        /// Get dashboard by ID
        /// </summary>
        Task<Dashboards> GetByIdAsync(int id);

        /// <summary>
        /// Get all dashboards
        /// </summary>
        Task<IEnumerable<Dashboards>> GetAllAsync();

        /// <summary>
        /// Add a new dashboard
        /// </summary>
        Task AddAsync(Dashboards dashboard);

        /// <summary>
        /// Update an existing dashboard
        /// </summary>
        Task UpdateAsync(Dashboards dashboard);

        /// <summary>
        /// Delete a dashboard
        /// </summary>
        Task DeleteAsync(Dashboards dashboard);
    }
}
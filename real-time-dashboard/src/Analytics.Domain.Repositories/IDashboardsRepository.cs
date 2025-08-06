// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    public interface IDashboardsRepository
    {
        Task<Dashboards?> GetByIdAsync(string id);
        Task AddAsync(Dashboards entity);
        Task UpdateAsync(Dashboards entity);
        Task DeleteAsync(string id);
        Task<IEnumerable<Dashboards>> GetAllAsync();
    }
}
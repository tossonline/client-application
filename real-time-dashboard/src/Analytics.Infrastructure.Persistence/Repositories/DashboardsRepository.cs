// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;

namespace Analytics.Infrastructure.Persistence.Repositories
{
    public sealed class DashboardsRepository : IDashboardsRepository
    {
        private readonly DbSet<Dashboards> _dbSet;
        private readonly DbContext _context;

        public DashboardsRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<Dashboards>();
        }

        public async Task<Dashboards?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(Dashboards entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(Dashboards entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<IEnumerable<Dashboards>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
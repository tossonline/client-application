// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Repository.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;

namespace Analytics.Infrastructure.Persistence.Repositories
{
    public sealed class DashboardsRepository : RepositoryBase<string, Dashboards>, IDashboardsRepository
    {
        public DashboardsRepository(DbSet<Dashboards> aggregateRoots)
            : base(aggregateRoots)
        {
        }
    }
}
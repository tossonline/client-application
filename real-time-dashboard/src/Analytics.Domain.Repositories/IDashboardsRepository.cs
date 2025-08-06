// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Repository.Abstractions;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    public interface IDashboardsRepository : IRepository<string, Dashboards>
    {
    }
}
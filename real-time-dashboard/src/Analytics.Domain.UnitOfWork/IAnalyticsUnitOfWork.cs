// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.UnitOfWork.Abstractions;
using Analytics.Domain.Repositories;

namespace Analytics.Domain.UnitOfWork
{
    public interface IAnalyticsUnitOfWork : IUnitOfWork
    {
        IDashboardsRepository DashboardsRepository { get; set; }
    }
}
// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Repositories;

namespace Analytics.Domain.UnitOfWork
{
    public interface IAnalyticsUnitOfWork
    {
        IDashboardsRepository DashboardsRepository { get; set; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Dispose();
    }
}
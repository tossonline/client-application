// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Observability.Messages;
using Analytics.Domain.Repositories;
using Analytics.Domain.UnitOfWork;
using Analytics.Infrastructure.Persistence.Contexts;
using Analytics.Infrastructure.Persistence.Exceptions;
using Analytics.Infrastructure.Persistence.Repositories;

namespace Analytics.Infrastructure.Persistence.UnitOfWork
{
    public sealed class AnalyticsUnitOfWork : IAnalyticsUnitOfWork
    {
        private readonly AnalyticsContext _context;

        public AnalyticsUnitOfWork(AnalyticsContext context)
        {
            _context = context;

            DashboardsRepository = new DashboardsRepository(_context.Dashboardss);
        }

        public IDashboardsRepository DashboardsRepository { get; set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw HandleException(ex);
            }
        }

        private static Exception HandleException(Exception exception)
        {
            if (exception is not DbUpdateException updateException)
            {
                return exception;
            }

            if (updateException.InnerException is not SqlException sqlException)
            {
                return exception;
            }

            switch (sqlException.Number)
            {
                case 2627:
                case 547:
                    throw new UniqueConstraintException(ErrorMessages.ENTITY_FRAMEWORK_UNIQUE_CONSTRAINT_ERROR);
                case 2601:
                    throw new DuplicatedKeyRowException(ErrorMessages.ENTITY_FRAMEWORK_UNIQUE_ROW_KEY_ERROR);
                default:
                    throw new RepositoryException(ErrorMessages.ENTITY_FRAMEWORK_SAVE_CHANGES_ERROR);
            }
        }
    }
}
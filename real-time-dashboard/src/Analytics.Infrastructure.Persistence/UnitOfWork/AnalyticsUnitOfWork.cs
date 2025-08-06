// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        private IDbContextTransaction? _transaction;

        public AnalyticsUnitOfWork(AnalyticsContext context)
        {
            _context = context;
            DashboardsRepository = new DashboardsRepository(_context);
        }

        public IDashboardsRepository DashboardsRepository { get; set; }

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

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
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
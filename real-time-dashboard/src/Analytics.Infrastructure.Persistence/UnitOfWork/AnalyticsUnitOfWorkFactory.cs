// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Observability.Messages;
using Analytics.Domain.UnitOfWork;
using Analytics.Infrastructure.Persistence.Contexts;
using Analytics.Infrastructure.Persistence.Exceptions;

namespace Analytics.Infrastructure.Persistence.UnitOfWork
{
    public sealed class AnalyticsUnitOfWorkFactory : IUnitOfWorkFactory<IAnalyticsUnitOfWork>
    {
        private readonly IDbContextFactory<AnalyticsContext> _contextFactory;

        public AnalyticsUnitOfWorkFactory(IDbContextFactory<AnalyticsContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IAnalyticsUnitOfWork BuildUnitOfWork()
        {
            if (_contextFactory == null)
            {
                throw new UnitOfWorkException(ErrorMessages.UNIT_OF_WORK_CANNOT_BE_NULL);
            }

            AnalyticsContext context = _contextFactory.CreateDbContext();
            context.ChangeTracker.LazyLoadingEnabled = false;

            return new AnalyticsUnitOfWork(context);
        }
    }
}
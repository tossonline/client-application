// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Domain.Models.Configuration.SqlServer;
using Analytics.Domain.UnitOfWork;
using Analytics.Infrastructure.Persistence.Contexts;
using Analytics.Infrastructure.Persistence.UnitOfWork;

namespace Analytics.Infrastructure.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            // Register our own UnitOfWork factory implementation
            return services.AddSingleton<IAnalyticsUnitOfWorkFactory, AnalyticsUnitOfWorkFactory>();
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, SqlServerConfiguration sqlServerConfiguration)
        {
            return services
                .AddMemoryCache()
                .AddDbContextFactory<AnalyticsContext>(options =>
                    options.UseSqlServer(sqlServerConfiguration.ConnectionString, dbOptions => { dbOptions.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds); }));
        }

        public static IServiceCollection AddMockDatabase(this IServiceCollection services, string databaseName)
        {
            return services.AddDbContextFactory<AnalyticsContext>(options => { options.UseInMemoryDatabase(databaseName); });
        }
    }
}
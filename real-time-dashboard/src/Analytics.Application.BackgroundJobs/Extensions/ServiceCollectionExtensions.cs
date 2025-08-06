// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.BackgroundServices.Extensions;
using Affiliate.Platform.Extensions.BackgroundServices.Options;
using Affiliate.Platform.Extensions.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Domain.Models.Configuration;

namespace Analytics.Application.BackgroundJobs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IAnalyticsConfiguration serviceConfiguration)
        {
            return services
                .AddBackgroundServices(serviceConfiguration);
        }

        public static IApplicationBuilder AddBackgroundServicesDashboard(this IApplicationBuilder applicationBuilder, IAnalyticsConfiguration serviceConfiguration)
        {
            return applicationBuilder
                .AddBackgroundServicesDashboard(BuildBackgroundServicesOptions(serviceConfiguration));
        }

        private static IServiceCollection AddBackgroundServices(this IServiceCollection services, IAnalyticsConfiguration serviceConfiguration)
        {
            return services
                .AddBackgroundServicesWithSqlPersistence(BuildBackgroundServicesOptions(serviceConfiguration));
        }

        private static BackgroundServicesOptions BuildBackgroundServicesOptions(IAnalyticsConfiguration serviceConfiguration)
        {
            return new BackgroundServicesOptions
            {
                DashboardOptions = new BackgroundServicesDashboardOptions
                {
                    Title = serviceConfiguration.BackgroundServices.Dashboard.Title,
                    Path = serviceConfiguration.BackgroundServices.Dashboard.Path,
                    Authentication = new BackgroundServicesAuthOptions
                    {
                        Username = serviceConfiguration.BackgroundServices.Dashboard.Username,
                        Password = serviceConfiguration.BackgroundServices.Dashboard.Password
                    }
                },
                PersistenceOptions = new BackgroundServicesPersistenceOptions
                {
                    Database = serviceConfiguration.Persistence.Database,
                    Encrypt = serviceConfiguration.Persistence.Encrypt,
                    Password = serviceConfiguration.Persistence.Password,
                    Server = serviceConfiguration.Persistence.Server,
                    Username = serviceConfiguration.Persistence.Username
                }
            };
        }
    }
}
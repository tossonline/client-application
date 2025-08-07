// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Service.Configuration;
using Analytics.Application.BackgroundJobs.Extensions;
using Analytics.Domain.Models.Configuration;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class MiddlewareRegistration
    {
        public static void RegisterDependencies(IApplicationBuilder applicationBuilder, IServiceConfiguration serviceConfiguration)
        {
            applicationBuilder
                .AddBackgroundServicesDashboard((IAnalyticsConfiguration)serviceConfiguration);
        }
    }
}
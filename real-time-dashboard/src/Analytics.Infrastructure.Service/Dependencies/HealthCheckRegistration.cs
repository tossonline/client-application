// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Application.HealthChecks.Extensions;
using Analytics.Domain.Models.Configuration;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class HealthCheckRegistration
    {
        public static void RegisterDependencies(IHealthChecksBuilder builder, IAnalyticsConfiguration serviceConfiguration)
        {
            builder
                .AddHealthChecks(serviceConfiguration);
        }
    }
}
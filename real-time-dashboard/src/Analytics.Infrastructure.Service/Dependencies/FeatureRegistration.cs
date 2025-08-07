// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Logging.Messaging.Extensions;
using Affiliate.Platform.Extensions.Service.Configuration;
using Analytics.Domain.Models.Configuration;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class FeatureRegistration
    {
        public static void RegisterDependencies(IServiceCollection service, IServiceConfiguration serviceConfiguration)
        {
            service
                .AddLogging((IAnalyticsConfiguration)serviceConfiguration);
        }

        private static void AddLogging(this IServiceCollection service, IAnalyticsConfiguration serviceConfiguration)
        {
            service.AddLogging(builder => builder.AddMessagingLogger(options =>
            {
                options.Team = serviceConfiguration.LoggerOptions.Team;
                options.Environment = serviceConfiguration.LoggerOptions.Environment;
                options.ApplicationType = serviceConfiguration.LoggerOptions.ApplicationType;
                options.Application = serviceConfiguration.LoggerOptions.Application;
                options.Source = serviceConfiguration.LoggerOptions.Source;
            }));
        }
    }
}
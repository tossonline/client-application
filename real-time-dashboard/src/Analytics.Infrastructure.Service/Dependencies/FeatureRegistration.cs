// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class FeatureRegistration
    {
        public static void RegisterDependencies(IServiceCollection service, IAnalyticsConfiguration serviceConfiguration)
        {
            service
                .AddLogging(serviceConfiguration);
        }

        private static void AddLogging(this IServiceCollection service, IAnalyticsConfiguration serviceConfiguration)
        {
            service.AddLogging(builder => 
            {
                builder.AddConsole();
                builder.AddDebug();
                // Additional logging configuration can be added here
            });
        }
    }
}
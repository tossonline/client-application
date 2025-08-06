// Copyright (c) DigiOutsource. All rights reserved.

using System.Diagnostics;
using Affiliate.Platform.Extensions.Observability;
using Affiliate.Platform.Extensions.Tracing.Extensions;
using Affiliate.Platform.Extensions.Tracing.Options;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Application.Handlers.Extensions;
using Analytics.Application.Translators.Extensions;
using Analytics.Domain.Models.Configuration;
using Analytics.Infrastructure.Persistence.Extensions;

namespace Analytics.Tests.Shared
{
    public static class DependencyInjection
    {
        public static ServiceProvider Setup()
        {
            IServiceCollection services = new ServiceCollection();

            ObservabilityManagerConfiguration.Service = "analytics";
            ObservabilityManagerConfiguration.Context = "affiliate";

            ActivitySource.AddActivityListener(new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
            });

            services
                .AddSingleton<IAnalyticsConfiguration>(new AnalyticsConfiguration());

            services
                .AddMetrics();

            services
                .AddLogging();

            services
                .AddDistributedTracing(new TracingOptions
                {
                    Host = "localhost",
                    Port = 4317,
                    Source = "Affiliate.Platform.Affiliate.Analytics.Service.Tests",
                    ServiceName = "Affiliate.Platform.Affiliate.Analytics"
                });

            services
                .AddUnitOfWork();

            services
                .AddHandlers();

            services
                .AddTranslators();

            services
                .AddMockDatabase($"dbAffiliate_Analytics_{Guid.NewGuid()}"); //Guid is added here to force a new DB context and no data to be reused

            return services.BuildServiceProvider();
        }
    }
}
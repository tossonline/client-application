// Copyright (c) DigiOutsource. All rights reserved.

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            // Replace custom observability with standard logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            ActivitySource.AddActivityListener(new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
            });

            services
                .AddSingleton<IAnalyticsConfiguration>(new AnalyticsConfiguration());

            services
                .AddUnitOfWork();

            services
                .AddHandlers();

            services
                .AddTranslators();

            services
                .AddMockDatabase($"dbAnalytics_{Guid.NewGuid()}"); //Guid is added here to force a new DB context and no data to be reused

            return services.BuildServiceProvider();
        }
    }
}
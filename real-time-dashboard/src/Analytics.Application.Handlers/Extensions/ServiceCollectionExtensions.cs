// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Messaging.Abstractions.Extensions;
using Affiliate.Platform.Metrics.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Application.Handlers.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            //TODO: Add Handler injection here
            return services;
        }

        public static IMetricsBuilder AddHandlerMetrics(this IMetricsBuilder metricsBuilder)
        {
            return metricsBuilder
                .AddHandlerBaseMetrics();
        }
    }
}
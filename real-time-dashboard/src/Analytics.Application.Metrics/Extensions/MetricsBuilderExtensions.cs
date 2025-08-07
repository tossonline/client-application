// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Observability.Extensions;
using Affiliate.Platform.Metrics.Abstractions;
using Analytics.Domain.Observability.Names;

namespace Analytics.Application.Metrics.Extensions
{
    public static class MetricsBuilderExtensions
    {
        public static IMetricsBuilder AddDomainMetrics(this IMetricsBuilder builder)
        {
            return builder
                .AddCounter(MetricMeasureNames.CounterNumberOfArchiveExecutions)
                .AddCounter(MetricMeasureNames.CounterNumberOfArchiveCleanupExecutions)
                .AddCounter(MetricMeasureNames.CounterNumberOfBackgroundJobExceptions);
        }
    }
}
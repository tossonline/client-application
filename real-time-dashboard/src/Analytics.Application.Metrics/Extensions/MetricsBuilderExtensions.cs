// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Observability.Names;

namespace Analytics.Application.Metrics.Extensions
{
    public static class MetricsBuilderExtensions
    {
        // Stub for metrics registration using prometheus-net or other public library
        public static void AddDomainMetrics(/* metrics registry or builder */)
        {
            // Example: register counters using prometheus-net
            // Metrics.CreateCounter(MetricMeasureNames.CounterNumberOfArchiveExecutions.Name, "Description");
            // Metrics.CreateCounter(MetricMeasureNames.CounterNumberOfArchiveCleanupExecutions.Name, "Description");
            // Metrics.CreateCounter(MetricMeasureNames.CounterNumberOfBackgroundJobExceptions.Name, "Description");
        }
    }
}
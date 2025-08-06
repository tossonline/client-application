// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration.BackgroundServices.Archiving;
using Analytics.Domain.Models.Configuration.BackgroundServices.Cleanup;
using Analytics.Domain.Models.Configuration.BackgroundServices.Metrics;

namespace Analytics.Domain.Models.Configuration.BackgroundServices
{
    [Serializable]
    public sealed class JobConfigurations
    {
        public JobConfigurations()
        {
            MetricsJobConfiguration = new Dictionary<string, MetricJobConfiguration>();
            ArchivingJobConfiguration = new Dictionary<string, ArchivingJobConfiguration>();
            CleanupJobConfiguration = new Dictionary<string, CleanupJobConfiguration>();
        }
        
        public Dictionary<string, MetricJobConfiguration>? MetricsJobConfiguration { get; init; }
        public Dictionary<string, ArchivingJobConfiguration>? ArchivingJobConfiguration { get; init; }
        public Dictionary<string, CleanupJobConfiguration>? CleanupJobConfiguration { get; init; }
    }
}
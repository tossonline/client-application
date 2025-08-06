// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration.SqlServer;

namespace Analytics.Domain.Models.Configuration.BackgroundServices.Metrics
{
    [Serializable]
    public sealed class MetricJobConfiguration
    {
        public MetricJobConfiguration()
        {
            Schedule = string.Empty;
            Source = new SqlServerConfiguration();
            JobStoredProcedures = new Dictionary<string, string>();
        }
        
        public string Schedule { get; init; }

        public SqlServerConfiguration Source { get; init; }

        public Dictionary<string, string> JobStoredProcedures { get; init; }
    }
}
// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.Configuration.SqlServer;

namespace Analytics.Domain.Models.Configuration.BackgroundServices.Cleanup
{
    public sealed class CleanupJobConfiguration
    {
        public CleanupJobConfiguration()
        {
            Schedule = string.Empty;
            DestinationDatabase = string.Empty;
            Source = new SqlServerConfiguration();
            RetentionPeriodDays = 30;
            BatchSize = 10000;
            BatchDelay = "00:00:05";
            JobStoredProcedures = new Dictionary<string, string>();
        }
        
        public string Schedule { get; init; }

        public string DestinationDatabase { get; init; }

        public int RetentionPeriodDays { get; init; }

        public int BatchSize { get; init; }

        public string BatchDelay { get; init; }

        public SqlServerConfiguration Source { get; init; }

        public Dictionary<string, string> JobStoredProcedures { get; init; }
    }
}
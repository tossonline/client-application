// Copyright (c) DigiOutsource. All rights reserved.

using System.Collections.Generic;

namespace Analytics.Domain.Observability.Names
{
    public static class MetricMeasureNames
    {
        static MetricMeasureNames()
        {
            CounterNumberOfBackgroundJobExceptions =
                new ObservableMetric("background_job_exceptions", "Count of the number of exceptions that happened on the background jobs.");
            CounterNumberOfArchiveExecutions =
                new ObservableMetric("archive_executions", "Count of the number of times the archive job has been executed.");
            CounterNumberOfArchiveCleanupExecutions =
                new ObservableMetric("archive_cleanup_executions", "Count of the number of times the archive cleanup job has been executed.");
            GaugeDeadLetterQueueItems =
                new ObservableMetric("dead_letter_queue_item_count", "Number of items in the dead letter queue.", new List<string> { "dead_letter_queue_name" });
        }

        public static ObservableMetric CounterNumberOfBackgroundJobExceptions { get; }
        public static ObservableMetric CounterNumberOfArchiveExecutions { get; }
        public static ObservableMetric CounterNumberOfArchiveCleanupExecutions { get; }
        public static ObservableMetric GaugeDeadLetterQueueItems { get; }
    }

    public class ObservableMetric
    {
        public string Name { get; }
        public string Description { get; }
        public List<string> Labels { get; }

        public ObservableMetric(string name, string description, List<string> labels = null)
        {
            Name = name;
            Description = description;
            Labels = labels ?? new List<string>();
        }
    }
}
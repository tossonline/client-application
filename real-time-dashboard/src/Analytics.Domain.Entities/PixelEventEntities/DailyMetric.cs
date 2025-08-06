using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents daily aggregated metrics for an event type.
    /// </summary>
    public class DailyMetric
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public int Count { get; set; }
    }
}

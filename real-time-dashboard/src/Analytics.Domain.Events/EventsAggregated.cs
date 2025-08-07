using System;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when events are aggregated for a specific date
    /// </summary>
    public class EventsAggregated
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerTag { get; set; }
        public int Count { get; set; }
        public DateTime AggregatedAt { get; set; }
    }
}
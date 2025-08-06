using System;
using System.Collections.Generic;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when events are aggregated for a specific time period
    /// </summary>
    public class EventsAggregated
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EventType { get; set; }
        public string BannerTag { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> DailyCounts { get; set; }
        public DateTime AggregatedAt { get; set; }

        public EventsAggregated()
        {
            DailyCounts = new Dictionary<string, 
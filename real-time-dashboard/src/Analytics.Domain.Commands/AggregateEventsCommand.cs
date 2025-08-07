using System;

namespace Analytics.Domain.Commands
{
    /// <summary>
    /// Command to aggregate events for a specific date
    /// </summary>
    public class AggregateEventsCommand
    {
        public DateTime EventDate { get; set; }
        public string? EventType { get; set; }
        public string? BannerTag { get; set; }
    }
} 
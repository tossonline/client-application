using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents aggregated event summary for analytics
    /// </summary>
    public class EventSummary
    {
        public int Id { get; set; }
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerTag { get; set; }
        public int Count { get; set; }
    }
}

using System;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents daily aggregated metrics per campaign/banner.
    /// </summary>
    public class EventSummary : IEventSummary
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string BannerTag { get; set; }
        public int Count { get; set; }
    }
}

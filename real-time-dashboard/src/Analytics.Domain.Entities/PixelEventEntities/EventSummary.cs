using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents daily aggregated metrics per campaign/banner.
    /// </summary>
    public class EventSummary
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string BannerTag { get; set; }
        public int Count { get; set; }
    }
}

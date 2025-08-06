using System;

namespace Analytics.Domain.Commands
{
    /// <summary>
    /// Command to aggregate events for a specific date range
    /// </summary>
    public class AggregateEventsCommand
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EventType { get; set; }
        public string BannerTag { get; set; }

        public AggregateEventsCommand()
        {
        }

        public AggregateEventsCommand(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        public bool IsValid()
        {
            return FromDate <= ToDate &&
                   FromDate >= DateTime.Today.AddDays(-365) && // Don't allow queries older than 1 year
                   ToDate <= DateTime.Today.AddDays(1); // Don't allow future dates beyond tomorrow
        }
    }
} 
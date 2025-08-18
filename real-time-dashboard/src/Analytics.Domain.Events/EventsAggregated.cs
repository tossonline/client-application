using System;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when events are aggregated for a specific date
    /// </summary>
    public class EventsAggregated : DomainEvent
    {
        /// <summary>
        /// Gets the date for which events were aggregated
        /// </summary>
        public DateTime EventDate { get; }

        /// <summary>
        /// Gets the type of events that were aggregated
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// Gets the banner tag for which events were aggregated, if any
        /// </summary>
        public string? BannerTag { get; }

        /// <summary>
        /// Gets the total count of events that were aggregated
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the timestamp when the aggregation was completed
        /// </summary>
        public DateTime AggregatedAt { get; }

        /// <summary>
        /// Initializes a new instance of the EventsAggregated class
        /// </summary>
        /// <param name="eventDate">The date for which events were aggregated</param>
        /// <param name="eventType">The type of events that were aggregated</param>
        /// <param name="bannerTag">The banner tag for which events were aggregated</param>
        /// <param name="count">The total count of events that were aggregated</param>
        /// <param name="aggregatedAt">The timestamp when the aggregation was completed</param>
        public EventsAggregated(
            DateTime eventDate,
            string eventType,
            string? bannerTag,
            int count,
            DateTime aggregatedAt)
        {
            EventDate = eventDate;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            BannerTag = bannerTag;
            Count = count;
            AggregatedAt = aggregatedAt;
        }
    }
}
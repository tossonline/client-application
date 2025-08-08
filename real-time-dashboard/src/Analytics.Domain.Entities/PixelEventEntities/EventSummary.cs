using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents aggregated event summary for analytics.
    /// This entity stores aggregated metrics for events over specific time periods.
    /// </summary>
    /// <remarks>
    /// Event summaries are used to efficiently query analytics data without processing
    /// individual events. They are automatically updated as new events are processed
    /// and provide a foundation for real-time analytics dashboards.
    /// </remarks>
    public class EventSummary
    {
        /// <summary>
        /// Gets the unique identifier for the summary
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the date for which this summary is calculated
        /// </summary>
        public DateTime EventDate { get; private set; }

        /// <summary>
        /// Gets the type of event being summarized
        /// </summary>
        public string EventType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the banner tag associated with the events, if any
        /// </summary>
        public string? BannerTag { get; private set; }

        /// <summary>
        /// Gets the total count of events for this summary
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the time period for which this summary is calculated
        /// </summary>
        public TimePeriod Period { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private EventSummary() { }

        /// <summary>
        /// Creates a new event summary
        /// </summary>
        /// <param name="date">The date for the summary</param>
        /// <param name="eventType">The type of event being summarized</param>
        /// <param name="bannerTag">Optional. The banner tag to summarize events for</param>
        /// <param name="period">The time period for the summary</param>
        /// <returns>A new EventSummary instance</returns>
        /// <exception cref="ArgumentException">Thrown when eventType is null or empty</exception>
        public static EventSummary Create(DateTime date, string eventType, string? bannerTag, TimePeriod period)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));

            return new EventSummary
            {
                EventDate = date.Date,
                EventType = eventType,
                BannerTag = bannerTag,
                Period = period,
                Count = 0
            };
        }

        /// <summary>
        /// Increments the event count
        /// </summary>
        /// <param name="amount">The amount to increment by. Defaults to 1</param>
        /// <exception cref="ArgumentException">Thrown when amount is not positive</exception>
        public void IncrementCount(int amount = 1)
        {
            if (amount <= 0)
                throw new ArgumentException("Increment amount must be positive", nameof(amount));

            Count += amount;
        }

        /// <summary>
        /// Decrements the event count
        /// </summary>
        /// <param name="amount">The amount to decrement by. Defaults to 1</param>
        /// <exception cref="ArgumentException">Thrown when amount is not positive</exception>
        /// <exception cref="InvalidOperationException">Thrown when decrementing would result in a negative count</exception>
        public void DecrementCount(int amount = 1)
        {
            if (amount <= 0)
                throw new ArgumentException("Decrement amount must be positive", nameof(amount));

            if (Count - amount < 0)
                throw new InvalidOperationException("Cannot decrement count below zero");

            Count -= amount;
        }

        /// <summary>
        /// Merges another summary into this one
        /// </summary>
        /// <param name="other">The summary to merge</param>
        /// <exception cref="ArgumentNullException">Thrown when other is null</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when summaries have different event types, banner tags, or dates
        /// </exception>
        /// <remarks>
        /// This method is used to combine summaries, typically when aggregating data
        /// from different sources or time periods
        /// </remarks>
        public void Merge(EventSummary other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.EventType != EventType || other.BannerTag != BannerTag || other.EventDate != EventDate)
                throw new ArgumentException("Cannot merge summaries with different event types, banner tags, or dates");

            Count += other.Count;
        }
    }

    /// <summary>
    /// Represents the time period for event aggregation
    /// </summary>
    public enum TimePeriod
    {
        /// <summary>
        /// Hourly aggregation
        /// </summary>
        Hourly,

        /// <summary>
        /// Daily aggregation
        /// </summary>
        Daily,

        /// <summary>
        /// Weekly aggregation
        /// </summary>
        Weekly,

        /// <summary>
        /// Monthly aggregation
        /// </summary>
        Monthly
    }
}
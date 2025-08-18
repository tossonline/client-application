using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Services.Events
{
    /// <summary>
    /// Interface for event aggregation service
    /// </summary>
    public interface IEventAggregationService
    {
        /// <summary>
        /// Aggregates events for a specific date
        /// </summary>
        /// <param name="eventDate">The date to aggregate events for</param>
        /// <returns>Collection of event summaries</returns>
        Task<IEnumerable<EventSummary>> AggregateEventsForDateAsync(DateTime eventDate);

        /// <summary>
        /// Aggregates events for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Collection of event summaries</returns>
        Task<IEnumerable<EventSummary>> AggregateEventsForDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Aggregates events by campaign for a specific date
        /// </summary>
        /// <param name="eventDate">The date to aggregate events for</param>
        /// <returns>Collection of campaign-specific event summaries</returns>
        Task<IEnumerable<EventSummary>> AggregateEventsByCampaignAsync(DateTime eventDate);

        /// <summary>
        /// Gets aggregated metrics for a specific date
        /// </summary>
        /// <param name="eventDate">The date to get metrics for</param>
        /// <returns>Daily metrics for the specified date</returns>
        Task<DailyMetric> GetDailyMetricsAsync(DateTime eventDate);
    }
}

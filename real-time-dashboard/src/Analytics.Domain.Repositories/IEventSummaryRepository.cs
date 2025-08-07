using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    /// <summary>
    /// Repository interface for EventSummary entity operations
    /// </summary>
    public interface IEventSummaryRepository
    {
        /// <summary>
        /// Add a new event summary
        /// </summary>
        Task AddAsync(EventSummary eventSummary);

        /// <summary>
        /// Get event summaries by date range
        /// </summary>
        Task<IEnumerable<EventSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get event summaries by date and event type
        /// </summary>
        Task<IEnumerable<EventSummary>> GetByDateAndEventTypeAsync(DateTime date, string eventType);

        /// <summary>
        /// Get event summaries by date and banner tag
        /// </summary>
        Task<IEnumerable<EventSummary>> GetByDateAndBannerTagAsync(DateTime date, string bannerTag);

        /// <summary>
        /// Get event summaries by date, event type, and banner tag
        /// </summary>
        Task<IEnumerable<EventSummary>> GetByDateEventTypeAndBannerTagAsync(DateTime date, string eventType, string bannerTag);

        /// <summary>
        /// Get all event summaries for a specific date
        /// </summary>
        Task<IEnumerable<EventSummary>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Update an existing event summary
        /// </summary>
        Task UpdateAsync(EventSummary eventSummary);

        /// <summary>
        /// Delete event summaries by date
        /// </summary>
        Task DeleteByDateAsync(DateTime date);
    }
}


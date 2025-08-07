using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    /// <summary>
    /// Repository interface for PixelEvent entity operations
    /// </summary>
    public interface IPixelEventRepository
    {
        /// <summary>
        /// Get pixel event by ID
        /// </summary>
        Task<PixelEvent> GetByIdAsync(int id);

        /// <summary>
        /// Get all pixel events
        /// </summary>
        Task<IEnumerable<PixelEvent>> GetAllAsync();

        /// <summary>
        /// Get pixel events by date
        /// </summary>
        Task<IEnumerable<PixelEvent>> GetByDateAsync(DateTime date);

        /// <summary>
        /// Get pixel events by date range
        /// </summary>
        Task<IEnumerable<PixelEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get pixel events by player ID
        /// </summary>
        Task<IEnumerable<PixelEvent>> GetByPlayerIdAsync(string playerId);

        /// <summary>
        /// Get pixel events by event type
        /// </summary>
        Task<IEnumerable<PixelEvent>> GetByEventTypeAsync(string eventType);

        /// <summary>
        /// Get pixel events by banner tag
        /// </summary>
        Task<IEnumerable<PixelEvent>> GetByBannerTagAsync(string bannerTag);

        /// <summary>
        /// Add a new pixel event
        /// </summary>
        Task AddAsync(PixelEvent pixelEvent);

        /// <summary>
        /// Update an existing pixel event
        /// </summary>
        Task UpdateAsync(PixelEvent pixelEvent);

        /// <summary>
        /// Delete a pixel event
        /// </summary>
        Task DeleteAsync(PixelEvent pixelEvent);

        /// <summary>
        /// Check if pixel event exists
        /// </summary>
        Task<bool> ExistsAsync(int id);
    }
}

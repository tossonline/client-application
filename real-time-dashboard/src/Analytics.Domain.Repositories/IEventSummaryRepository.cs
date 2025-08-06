using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    public interface IEventSummaryRepository
    {
        Task<EventSummary> GetByDateAndTypeAsync(DateTime eventDate, string eventType, string bannerTag);
        Task<IEnumerable<EventSummary>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<EventSummary>> GetByEventTypeAsync(string eventType);
        Task<IEnumerable<EventSummary>> GetByBannerTagAsync(string bannerTag);
        Task UpsertAsync(EventSummary eventSummary);
        Task DeleteAsync(DateTime eventDate, string eventType, string bannerTag);
    }
} 
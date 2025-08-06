using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    public interface IEventSummaryRepository
    {
        Task<IEventSummary> GetByDateAndTypeAsync(DateTime eventDate, string eventType, string bannerTag);
        Task<IEnumerable<IEventSummary>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<IEventSummary>> GetByEventTypeAsync(string eventType);
        Task<IEnumerable<IEventSummary>> GetByBannerTagAsync(string bannerTag);
        Task UpsertAsync(IEventSummary eventSummary);
        Task DeleteAsync(DateTime eventDate, string eventType, string bannerTag);
    }

    public interface IEventSummary
    {
        DateTime EventDate { get; set; }
        string EventType { get; set; }
        string BannerTag { get; set; }
        int Count { get; set; }
    }
}
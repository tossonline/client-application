using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Abstractions;
using Analytics.Domain.Events;

namespace Analytics.Domain.Entities.MetricsAggregate
{
    /// <summary>
    /// Aggregate root for metrics and analytics calculations
    /// </summary>
    public class MetricsAggregate
    {
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly List<EventsAggregated> _domainEvents;

        public MetricsAggregate(
            IEventSummaryRepository eventSummaryRepository,
            IPixelEventRepository pixelEventRepository)
        {
            _eventSummaryRepository = eventSummaryRepository;
            _pixelEventRepository = pixelEventRepository;
            _domainEvents = new List<EventsAggregated>();
        }

        public async Task AggregateEventsAsync(DateTime fromDate, DateTime toDate, string eventType = null, string bannerTag = null)
        {
            // Get events for the date range
            var events = await _pixelEventRepository.GetByDateRangeAsync(fromDate, toDate);

            // Apply filters
            if (!string.IsNullOrEmpty(eventType))
            {
                events = events.Where(e => e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(bannerTag))
            {
                events = events.Where(e => e.BannerTag.Equals(bannerTag, StringComparison.OrdinalIgnoreCase));
            }

            // Group by date, event type, and banner tag
            var groupedEvents = events
                .GroupBy(e => new { e.CreatedAt.Date, e.EventType, e.BannerTag })
                .Select(g => new EventSummary
                {
                    EventDate = g.Key.Date,
                    EventType = g.Key.EventType,
                    BannerTag = g.Key.BannerTag,
                    Count = g.Count()
                })
                .ToList();

            // Save aggregated results
            foreach (var summary in groupedEvents)
            {
                await _eventSummaryRepository.UpsertAsync(summary);
            }

            // Raise domain event
            var domainEvent = new EventsAggregated
            {
                FromDate = fromDate,
                ToDate = toDate,
                EventType = eventType,
                BannerTag = bannerTag,
                TotalCount = groupedEvents.Sum(s => s.Count),
                DailyCounts = groupedEvents.ToDictionary(s => s.EventDate.ToString("yyyy-MM-dd"), s => s.Count)
            };

            _domainEvents.Add(domainEvent);
        }

        public async Task<List<EventSummary>> GetMetricsAsync(DateTime fromDate, DateTime toDate, string eventType = null, string bannerTag = null)
        {
            var summaries = await _eventSummaryRepository.GetByDateRangeAsync(fromDate, toDate);

            var filteredSummaries = summaries.AsEnumerable();

            if (!string.IsNullOrEmpty(eventType))
            {
                filteredSummaries = filteredSummaries.Where(s => s.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(bannerTag))
            {
                filteredSummaries = filteredSummaries.Where(s => s.BannerTag.Equals(bannerTag, StringComparison.OrdinalIgnoreCase));
            }

            return filteredSummaries.Cast<EventSummary>().ToList();
        }

        public async Task<ConversionMetrics> CalculateConversionMetricsAsync(DateTime fromDate, DateTime toDate, string bannerTag = null)
        {
            var summaries = await GetMetricsAsync(fromDate, toDate, bannerTag: bannerTag);

            var visits = summaries.Where(s => s.EventType == "visit").Sum(s => s.Count);
            var registrations = summaries.Where(s => s.EventType == "registration").Sum(s => s.Count);
            var deposits = summaries.Where(s => s.EventType == "deposit").Sum(s => s.Count);

            return new ConversionMetrics
            {
                FromDate = fromDate,
                ToDate = toDate,
                BannerTag = bannerTag,
                TotalVisits = visits,
                TotalRegistrations = registrations,
                TotalDeposits = deposits,
                VisitToRegistrationRate = visits > 0 ? (double)registrations / visits : 0,
                RegistrationToDepositRate = registrations > 0 ? (double)deposits / registrations : 0,
                VisitToDepositRate = visits > 0 ? (double)deposits / visits : 0
            };
        }

        public async Task<DailyMetrics> GetDailyMetricsAsync(DateTime date, string eventType = null, string bannerTag = null)
        {
            var summaries = await GetMetricsAsync(date, date, eventType, bannerTag);

            return new DailyMetrics
            {
                Date = date,
                EventType = eventType,
                BannerTag = bannerTag,
                TotalCount = summaries.Sum(s => s.Count),
                EventBreakdown = summaries.GroupBy(s => s.EventType)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Count)),
                BannerBreakdown = summaries.GroupBy(s => s.BannerTag)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Count))
            };
        }

        public IReadOnlyList<EventsAggregated> GetDomainEvents()
        {
            return _domainEvents.AsReadOnly();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    public class ConversionMetrics
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string BannerTag { get; set; }
        public int TotalVisits { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalDeposits { get; set; }
        public double VisitToRegistrationRate { get; set; }
        public double RegistrationToDepositRate { get; set; }
        public double VisitToDepositRate { get; set; }
    }

    public class DailyMetrics
    {
        public DateTime Date { get; set; }
        public string EventType { get; set; }
        public string BannerTag { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> EventBreakdown { get; set; } = new();
        public Dictionary<string, int> BannerBreakdown { get; set; } = new();
    }
}
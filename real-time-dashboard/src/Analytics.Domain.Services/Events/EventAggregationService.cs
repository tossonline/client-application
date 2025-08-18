using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Entities.Common;
using Analytics.Domain.Events;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Domain.Services.Events
{
    /// <summary>
    /// Domain service for aggregating events into summary metrics
    /// </summary>
    public class EventAggregationService : IEventAggregationService
    {
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly ILogger<EventAggregationService> _logger;

        public EventAggregationService(
            IPixelEventRepository pixelEventRepository,
            IEventSummaryRepository eventSummaryRepository,
            ILogger<EventAggregationService> logger)
        {
            _pixelEventRepository = pixelEventRepository ?? throw new ArgumentNullException(nameof(pixelEventRepository));
            _eventSummaryRepository = eventSummaryRepository ?? throw new ArgumentNullException(nameof(eventSummaryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Aggregates events for a specific date
        /// </summary>
        /// <param name="eventDate">The date to aggregate events for</param>
        /// <returns>Collection of event summaries</returns>
        public async Task<IEnumerable<EventSummary>> AggregateEventsForDateAsync(DateTime eventDate)
        {
            _logger.LogInformation("Starting event aggregation for date: {EventDate}", eventDate.Date);

            try
            {
                // Get events for the specified date
                var events = await _pixelEventRepository.GetByDateAsync(eventDate.Date);
                var eventList = events.ToList();

                _logger.LogInformation("Found {EventCount} events to aggregate for date: {EventDate}", 
                    eventList.Count, eventDate.Date);

                // Group events by type and banner tag
                var groupedEvents = eventList
                    .GroupBy(e => new { e.EventType, e.BannerTag })
                    .Select(g => new EventSummary
                    {
                        EventDate = eventDate.Date,
                        EventType = g.Key.EventType,
                        BannerTag = g.Key.BannerTag,
                        Count = g.Count(),
                        Period = TimePeriod.Daily
                    })
                    .ToList();

                // Save aggregated summaries
                foreach (var summary in groupedEvents)
                {
                    await _eventSummaryRepository.AddAsync(summary);
                }

                _logger.LogInformation("Successfully aggregated {SummaryCount} event summaries for date: {EventDate}", 
                    groupedEvents.Count, eventDate.Date);

                return groupedEvents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to aggregate events for date: {EventDate}", eventDate.Date);
                throw;
            }
        }

        /// <summary>
        /// Aggregates events for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Collection of event summaries</returns>
        public async Task<IEnumerable<EventSummary>> AggregateEventsForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Starting event aggregation for date range: {StartDate} to {EndDate}", 
                startDate.Date, endDate.Date);

            try
            {
                var allSummaries = new List<EventSummary>();

                // Aggregate for each date in the range
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var dailySummaries = await AggregateEventsForDateAsync(date);
                    allSummaries.AddRange(dailySummaries);
                }

                _logger.LogInformation("Successfully aggregated events for date range: {StartDate} to {EndDate}. Total summaries: {SummaryCount}", 
                    startDate.Date, endDate.Date, allSummaries.Count);

                return allSummaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to aggregate events for date range: {StartDate} to {EndDate}", 
                    startDate.Date, endDate.Date);
                throw;
            }
        }

        /// <summary>
        /// Aggregates events by campaign for a specific date
        /// </summary>
        /// <param name="eventDate">The date to aggregate events for</param>
        /// <returns>Collection of campaign-specific event summaries</returns>
        public async Task<IEnumerable<EventSummary>> AggregateEventsByCampaignAsync(DateTime eventDate)
        {
            _logger.LogInformation("Starting campaign-based event aggregation for date: {EventDate}", eventDate.Date);

            try
            {
                // Get events for the specified date
                var events = await _pixelEventRepository.GetByDateAsync(eventDate.Date);
                var eventList = events.ToList();

                // Group events by campaign ID (extracted from banner tag)
                var groupedEvents = eventList
                    .GroupBy(e => new { e.EventType, CampaignId = ExtractCampaignId(e.BannerTag) })
                    .Select(g => new EventSummary
                    {
                        EventDate = eventDate.Date,
                        EventType = g.Key.EventType,
                        BannerTag = g.Key.CampaignId, // Use campaign ID as banner tag
                        Count = g.Count(),
                        Period = TimePeriod.Daily
                    })
                    .ToList();

                // Save aggregated summaries
                foreach (var summary in groupedEvents)
                {
                    await _eventSummaryRepository.AddAsync(summary);
                }

                _logger.LogInformation("Successfully aggregated {SummaryCount} campaign-based event summaries for date: {EventDate}", 
                    groupedEvents.Count, eventDate.Date);

                return groupedEvents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to aggregate events by campaign for date: {EventDate}", eventDate.Date);
                throw;
            }
        }

        /// <summary>
        /// Gets aggregated metrics for a specific date
        /// </summary>
        /// <param name="eventDate">The date to get metrics for</param>
        /// <returns>Daily metrics for the specified date</returns>
        public async Task<DailyMetric> GetDailyMetricsAsync(DateTime eventDate)
        {
            _logger.LogInformation("Getting daily metrics for date: {EventDate}", eventDate.Date);

            try
            {
                // Get events for the specified date
                var events = await _pixelEventRepository.GetByDateAsync(eventDate.Date);
                var eventList = events.ToList();

                // Create daily metric
                var dailyMetric = DailyMetric.Create(eventDate.Date, "all");

                // Count events by type
                var visitCount = eventList.Count(e => e.EventType == EventType.Visit.Value);
                var registrationCount = eventList.Count(e => e.EventType == EventType.Registration.Value);
                var depositCount = eventList.Count(e => e.EventType == EventType.Deposit.Value);

                // Update counts
                dailyMetric.UpdateVisitCount(visitCount);
                dailyMetric.UpdateRegistrationCount(registrationCount);
                dailyMetric.UpdateDepositCount(depositCount);

                _logger.LogInformation("Daily metrics for {EventDate}: Visits={VisitCount}, Registrations={RegistrationCount}, Deposits={DepositCount}", 
                    eventDate.Date, visitCount, registrationCount, depositCount);

                return dailyMetric;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get daily metrics for date: {EventDate}", eventDate.Date);
                throw;
            }
        }

        /// <summary>
        /// Extracts campaign ID from banner tag
        /// </summary>
        /// <param name="bannerTag">The banner tag to parse</param>
        /// <returns>The extracted campaign ID</returns>
        private static string ExtractCampaignId(string bannerTag)
        {
            var parts = bannerTag.Split('-');
            return parts.Length > 0 ? parts[0] : bannerTag;
        }
    }
}

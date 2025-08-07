using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Application.Models.Response;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Services.RealTime
{
    public class RealTimeMetricsService : IRealTimeMetricsService
    {
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly ILogger<RealTimeMetricsService> _logger;
        private readonly ConcurrentDictionary<string, Func<RealTimeMetricsResponse, Task>> _subscribers;

        public RealTimeMetricsService(
            IPixelEventRepository pixelEventRepository,
            IEventSummaryRepository eventSummaryRepository,
            ILogger<RealTimeMetricsService> logger)
        {
            _pixelEventRepository = pixelEventRepository ?? throw new ArgumentNullException(nameof(pixelEventRepository));
            _eventSummaryRepository = eventSummaryRepository ?? throw new ArgumentNullException(nameof(eventSummaryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscribers = new ConcurrentDictionary<string, Func<RealTimeMetricsResponse, Task>>();
        }

        public async Task<RealTimeMetricsResponse> GetCurrentMetricsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var events = await _pixelEventRepository.GetByDateAsync(today);
            var summaries = await _eventSummaryRepository.GetByDateAsync(today);

            var hourlyBreakdown = events
                .GroupBy(e => e.Timestamp.Hour)
                .Select(g => new HourlyMetrics
                {
                    Hour = g.Key,
                    VisitCount = g.Count(e => e.EventType == "visit"),
                    RegistrationCount = g.Count(e => e.EventType == "registration"),
                    DepositCount = g.Count(e => e.EventType == "deposit"),
                    ConversionRate = CalculateRate(
                        g.Count(e => e.EventType == "registration"),
                        g.Count(e => e.EventType == "visit"))
                })
                .OrderBy(h => h.Hour)
                .ToList();

            var activeCampaigns = events
                .GroupBy(e => e.BannerTag)
                .Select(g => new ActiveCampaign
                {
                    BannerTag = g.Key,
                    VisitCount = g.Count(e => e.EventType == "visit"),
                    RegistrationCount = g.Count(e => e.EventType == "registration"),
                    DepositCount = g.Count(e => e.EventType == "deposit"),
                    ConversionRate = CalculateRate(
                        g.Count(e => e.EventType == "registration"),
                        g.Count(e => e.EventType == "visit")),
                    Performance = DeterminePerformance(g.ToList())
                })
                .ToList();

            var recentEvents = events
                .OrderByDescending(e => e.Timestamp)
                .Take(10)
                .Select(e => new RecentEvent
                {
                    Timestamp = e.Timestamp,
                    EventType = e.EventType,
                    BannerTag = e.BannerTag,
                    PlayerId = e.PlayerId
                })
                .ToList();

            var totalVisits = events.Count(e => e.EventType == "visit");
            var totalRegistrations = events.Count(e => e.EventType == "registration");
            var totalDeposits = events.Count(e => e.EventType == "deposit");

            var response = new RealTimeMetricsResponse
            {
                Timestamp = DateTime.UtcNow,
                TodayVisits = totalVisits,
                TodayRegistrations = totalRegistrations,
                TodayDeposits = totalDeposits,
                CurrentConversionRate = CalculateRate(totalRegistrations, totalVisits),
                CurrentDepositRate = CalculateRate(totalDeposits, totalRegistrations),
                HourlyBreakdown = hourlyBreakdown,
                ActiveCampaigns = activeCampaigns,
                RecentEvents = recentEvents
            };

            return response;
        }

        public async Task<RealTimeCampaignMetricsResponse> GetCampaignMetricsAsync(string bannerTag)
        {
            var today = DateTime.UtcNow.Date;
            var events = (await _pixelEventRepository.GetByDateAsync(today))
                .Where(e => e.BannerTag == bannerTag)
                .ToList();

            var hourlyBreakdown = events
                .GroupBy(e => e.Timestamp.Hour)
                .Select(g => new CampaignHourMetrics
                {
                    Hour = g.Key,
                    VisitCount = g.Count(e => e.EventType == "visit"),
                    RegistrationCount = g.Count(e => e.EventType == "registration"),
                    DepositCount = g.Count(e => e.EventType == "deposit"),
                    ConversionRate = CalculateRate(
                        g.Count(e => e.EventType == "registration"),
                        g.Count(e => e.EventType == "visit")),
                    DepositRate = CalculateRate(
                        g.Count(e => e.EventType == "deposit"),
                        g.Count(e => e.EventType == "registration"))
                })
                .OrderBy(h => h.Hour)
                .ToList();

            var recentEvents = events
                .OrderByDescending(e => e.Timestamp)
                .Take(10)
                .Select(e => new CampaignEvent
                {
                    Timestamp = e.Timestamp,
                    EventType = e.EventType,
                    PlayerId = e.PlayerId,
                    SourceIp = e.SourceIp,
                    UserAgent = e.UserAgent
                })
                .ToList();

            var totalVisits = events.Count(e => e.EventType == "visit");
            var totalRegistrations = events.Count(e => e.EventType == "registration");
            var totalDeposits = events.Count(e => e.EventType == "deposit");

            var avgTimeToRegistration = CalculateAverageTimeToEvent(events, "registration");
            var avgTimeToDeposit = CalculateAverageTimeToEvent(events, "deposit");

            var performance = AnalyzeCampaignPerformance(events);

            var response = new RealTimeCampaignMetricsResponse
            {
                BannerTag = bannerTag,
                Timestamp = DateTime.UtcNow,
                TodayMetrics = new CampaignDayMetrics
                {
                    VisitCount = totalVisits,
                    RegistrationCount = totalRegistrations,
                    DepositCount = totalDeposits,
                    ConversionRate = CalculateRate(totalRegistrations, totalVisits),
                    DepositRate = CalculateRate(totalDeposits, totalRegistrations),
                    AverageTimeToRegistration = avgTimeToRegistration,
                    AverageTimeToDeposit = avgTimeToDeposit
                },
                HourlyBreakdown = hourlyBreakdown,
                RecentEvents = recentEvents,
                Performance = performance
            };

            return response;
        }

        public Task SubscribeToUpdatesAsync(string connectionId, Func<RealTimeMetricsResponse, Task> callback)
        {
            _subscribers.TryAdd(connectionId, callback);
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromUpdatesAsync(string connectionId)
        {
            _subscribers.TryRemove(connectionId, out _);
            return Task.CompletedTask;
        }

        private decimal CalculateRate(int numerator, int denominator)
        {
            if (denominator == 0) return 0;
            return (decimal)numerator / denominator * 100;
        }

        private string DeterminePerformance(List<Domain.Entities.PixelEvent> events)
        {
            var visits = events.Count(e => e.EventType == "visit");
            if (visits == 0) return "No Data";

            var conversionRate = CalculateRate(
                events.Count(e => e.EventType == "registration"),
                visits);

            if (conversionRate >= 10) return "Performing Well";
            if (conversionRate >= 5) return "Average";
            return "Needs Attention";
        }

        private decimal CalculateAverageTimeToEvent(List<Domain.Entities.PixelEvent> events, string targetEventType)
        {
            var eventPairs = events
                .GroupBy(e => e.PlayerId)
                .Select(g =>
                {
                    var visit = g.FirstOrDefault(e => e.EventType == "visit");
                    var target = g.FirstOrDefault(e => e.EventType == targetEventType);
                    return new { visit, target };
                })
                .Where(pair => pair.visit != null && pair.target != null)
                .ToList();

            if (!eventPairs.Any()) return 0;

            var totalMinutes = eventPairs.Sum(pair =>
                (pair.target.Timestamp - pair.visit.Timestamp).TotalMinutes);

            return (decimal)(totalMinutes / eventPairs.Count);
        }

        private RealTimePerformance AnalyzeCampaignPerformance(List<Domain.Entities.PixelEvent> events)
        {
            var visits = events.Count(e => e.EventType == "visit");
            if (visits == 0)
            {
                return new RealTimePerformance
                {
                    Status = "No Data",
                    Alerts = new List<string> { "No visits recorded today" },
                    Recommendations = new List<string> { "Check campaign tracking setup" }
                };
            }

            var registrations = events.Count(e => e.EventType == "registration");
            var deposits = events.Count(e => e.EventType == "deposit");
            var conversionRate = CalculateRate(registrations, visits);
            var depositRate = CalculateRate(deposits, registrations);

            var alerts = new List<string>();
            var recommendations = new List<string>();
            var metrics = new Dictionary<string, decimal>
            {
                { "ConversionRate", conversionRate },
                { "DepositRate", depositRate }
            };

            string status;
            if (conversionRate >= 10 && depositRate >= 50)
            {
                status = "Performing Well";
                alerts.Add("Campaign is performing above targets");
                recommendations.Add("Consider increasing budget for this campaign");
            }
            else if (conversionRate >= 5 && depositRate >= 25)
            {
                status = "Average";
                alerts.Add("Campaign is performing within expected range");
                recommendations.Add("Monitor for optimization opportunities");
            }
            else
            {
                status = "Needs Attention";
                if (conversionRate < 5)
                {
                    alerts.Add("Low conversion rate");
                    recommendations.Add("Review landing page and user journey");
                }
                if (depositRate < 25)
                {
                    alerts.Add("Low deposit rate");
                    recommendations.Add("Evaluate deposit incentives");
                }
            }

            return new RealTimePerformance
            {
                Status = status,
                Alerts = alerts,
                Metrics = metrics,
                Recommendations = recommendations
            };
        }

        public async Task NotifySubscribersAsync()
        {
            try
            {
                var metrics = await GetCurrentMetricsAsync();
                var tasks = _subscribers.Values.Select(callback => callback(metrics));
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying subscribers of real-time metrics update");
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Application.Models.Alerts;
using Analytics.Application.Services.RealTime;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Services.Alerts
{
    public class AlertService : IAlertService
    {
        private readonly IRealTimeMetricsService _metricsService;
        private readonly ILogger<AlertService> _logger;
        private readonly ConcurrentDictionary<string, AlertSubscription> _subscriptions;
        private readonly ConcurrentDictionary<string, Alert> _activeAlerts;

        private readonly Dictionary<AlertType, Func<Task<List<Alert>>>> _alertChecks;

        public AlertService(
            IRealTimeMetricsService metricsService,
            ILogger<AlertService> logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptions = new ConcurrentDictionary<string, AlertSubscription>();
            _activeAlerts = new ConcurrentDictionary<string, Alert>();

            _alertChecks = new Dictionary<AlertType, Func<Task<List<Alert>>>>
            {
                { AlertType.ConversionRate, CheckConversionRateAlertsAsync },
                { AlertType.DepositRate, CheckDepositRateAlertsAsync },
                { AlertType.TrafficSpike, CheckTrafficSpikeAlertsAsync },
                { AlertType.TrafficDrop, CheckTrafficDropAlertsAsync },
                { AlertType.AnomalyDetected, CheckAnomalyAlertsAsync },
                { AlertType.PerformanceIssue, CheckPerformanceAlertsAsync },
                { AlertType.CampaignAlert, CheckCampaignAlertsAsync }
            };
        }

        public async Task<List<Alert>> CheckForAlertsAsync()
        {
            var newAlerts = new List<Alert>();

            try
            {
                foreach (var check in _alertChecks)
                {
                    var alerts = await check.Value();
                    foreach (var alert in alerts)
                    {
                        if (_activeAlerts.TryAdd(alert.Id, alert))
                        {
                            newAlerts.Add(alert);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for alerts");
            }

            return newAlerts;
        }

        public Task SubscribeToAlertsAsync(string connectionId, AlertSubscription subscription)
        {
            _subscriptions.TryAdd(connectionId, subscription);
            return Task.CompletedTask;
        }

        public Task UnsubscribeFromAlertsAsync(string connectionId)
        {
            _subscriptions.TryRemove(connectionId, out _);
            return Task.CompletedTask;
        }

        public Task AcknowledgeAlertAsync(string alertId, string userId)
        {
            if (_activeAlerts.TryGetValue(alertId, out var alert))
            {
                alert.Status = AlertStatus.Acknowledged;
                alert.Acknowledgment = new AlertAcknowledgment
                {
                    UserId = userId,
                    Timestamp = DateTime.UtcNow
                };
            }

            return Task.CompletedTask;
        }

        public Task<List<Alert>> GetActiveAlertsAsync()
        {
            var alerts = _activeAlerts.Values
                .Where(a => a.Status == AlertStatus.Active)
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.Timestamp)
                .ToList();

            return Task.FromResult(alerts);
        }

        public Task<List<Alert>> GetAlertHistoryAsync(AlertHistoryFilter filter)
        {
            var alerts = _activeAlerts.Values
                .Where(a => (!filter.StartDate.HasValue || a.Timestamp >= filter.StartDate.Value) &&
                           (!filter.EndDate.HasValue || a.Timestamp <= filter.EndDate.Value) &&
                           (filter.Types == null || filter.Types.Contains(a.Type)) &&
                           (filter.Severities == null || filter.Severities.Contains(a.Severity)) &&
                           (filter.Statuses == null || filter.Statuses.Contains(a.Status)) &&
                           (filter.Sources == null || filter.Sources.Contains(a.Source)))
                .OrderByDescending(a => a.Timestamp);

            if (filter.Limit.HasValue)
            {
                alerts = alerts.Take(filter.Limit.Value);
            }

            return Task.FromResult(alerts.ToList());
        }

        private async Task<List<Alert>> CheckConversionRateAlertsAsync()
        {
            var alerts = new List<Alert>();
            var metrics = await _metricsService.GetCurrentMetricsAsync();

            if (metrics.CurrentConversionRate < 5)
            {
                alerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = AlertType.ConversionRate,
                    Severity = AlertSeverity.Warning,
                    Title = "Low Conversion Rate",
                    Message = $"Current conversion rate ({metrics.CurrentConversionRate:F1}%) is below target (5%)",
                    Timestamp = DateTime.UtcNow,
                    Status = AlertStatus.Active,
                    SuggestedActions = new List<string>
                    {
                        "Review landing page performance",
                        "Check for technical issues",
                        "Analyze user journey funnel"
                    }
                });
            }

            return alerts;
        }

        private async Task<List<Alert>> CheckDepositRateAlertsAsync()
        {
            var alerts = new List<Alert>();
            var metrics = await _metricsService.GetCurrentMetricsAsync();

            if (metrics.CurrentDepositRate < 25)
            {
                alerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = AlertType.DepositRate,
                    Severity = AlertSeverity.Warning,
                    Title = "Low Deposit Rate",
                    Message = $"Current deposit rate ({metrics.CurrentDepositRate:F1}%) is below target (25%)",
                    Timestamp = DateTime.UtcNow,
                    Status = AlertStatus.Active,
                    SuggestedActions = new List<string>
                    {
                        "Review deposit incentives",
                        "Check payment gateway status",
                        "Analyze drop-off points in deposit flow"
                    }
                });
            }

            return alerts;
        }

        private async Task<List<Alert>> CheckTrafficSpikeAlertsAsync()
        {
            var alerts = new List<Alert>();
            var metrics = await _metricsService.GetCurrentMetricsAsync();

            var hourlyAverage = metrics.HourlyBreakdown
                .Where(h => h.Hour < DateTime.UtcNow.Hour)
                .Average(h => h.VisitCount);

            var currentHourVisits = metrics.HourlyBreakdown
                .FirstOrDefault(h => h.Hour == DateTime.UtcNow.Hour)?.VisitCount ?? 0;

            if (currentHourVisits > hourlyAverage * 2)
            {
                alerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = AlertType.TrafficSpike,
                    Severity = AlertSeverity.Info,
                    Title = "Traffic Spike Detected",
                    Message = $"Current hour traffic ({currentHourVisits} visits) is significantly higher than average ({hourlyAverage:F0} visits)",
                    Timestamp = DateTime.UtcNow,
                    Status = AlertStatus.Active,
                    SuggestedActions = new List<string>
                    {
                        "Monitor system performance",
                        "Check for traffic source changes",
                        "Ensure adequate system resources"
                    }
                });
            }

            return alerts;
        }

        private async Task<List<Alert>> CheckTrafficDropAlertsAsync()
        {
            var alerts = new List<Alert>();
            var metrics = await _metricsService.GetCurrentMetricsAsync();

            var hourlyAverage = metrics.HourlyBreakdown
                .Where(h => h.Hour < DateTime.UtcNow.Hour)
                .Average(h => h.VisitCount);

            var currentHourVisits = metrics.HourlyBreakdown
                .FirstOrDefault(h => h.Hour == DateTime.UtcNow.Hour)?.VisitCount ?? 0;

            if (currentHourVisits < hourlyAverage * 0.5)
            {
                alerts.Add(new Alert
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = AlertType.TrafficDrop,
                    Severity = AlertSeverity.Warning,
                    Title = "Traffic Drop Detected",
                    Message = $"Current hour traffic ({currentHourVisits} visits) is significantly lower than average ({hourlyAverage:F0} visits)",
                    Timestamp = DateTime.UtcNow,
                    Status = AlertStatus.Active,
                    SuggestedActions = new List<string>
                    {
                        "Check tracking pixel implementation",
                        "Verify campaign status",
                        "Monitor for technical issues"
                    }
                });
            }

            return alerts;
        }

        private async Task<List<Alert>> CheckAnomalyAlertsAsync()
        {
            var alerts = new List<Alert>();
            var metrics = await _metricsService.GetCurrentMetricsAsync();

            foreach (var campaign in metrics.ActiveCampaigns)
            {
                if (campaign.ConversionRate > 0 && campaign.VisitCount > 100)
                {
                    var avgConversionRate = metrics.ActiveCampaigns
                        .Where(c => c != campaign)
                        .Average(c => c.ConversionRate);

                    if (campaign.ConversionRate < avgConversionRate * 0.5)
                    {
                        alerts.Add(new Alert
                        {
                            Id = Guid.NewGuid().ToString(),
                            Type = AlertType.AnomalyDetected,
                            Severity = AlertSeverity.Warning,
                            Title = "Campaign Performance Anomaly",
                            Message = $"Campaign {campaign.BannerTag} conversion rate ({campaign.ConversionRate:F1}%) is significantly below average ({avgConversionRate:F1}%)",
                            Source = campaign.BannerTag,
                            Timestamp = DateTime.UtcNow,
                            Status = AlertStatus.Active,
                            SuggestedActions = new List<string>
                            {
                                "Review campaign targeting",
                                "Check landing page performance",
                                "Analyze user behavior data"
                            }
                        });
                    }
                }
            }

            return alerts;
        }

        private Task<List<Alert>> CheckPerformanceAlertsAsync()
        {
            // Implement performance monitoring alerts
            return Task.FromResult(new List<Alert>());
        }

        private Task<List<Alert>> CheckCampaignAlertsAsync()
        {
            // Implement campaign-specific alerts
            return Task.FromResult(new List<Alert>());
        }
    }
}

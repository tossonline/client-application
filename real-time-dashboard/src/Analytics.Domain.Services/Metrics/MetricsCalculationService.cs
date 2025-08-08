using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Domain.Services.Metrics
{
    public class MetricsCalculationService : IMetricsCalculationService
    {
        private readonly IPixelEventRepository _eventRepository;
        private readonly IEventSummaryRepository _summaryRepository;
        private readonly ILogger<MetricsCalculationService> _logger;

        public MetricsCalculationService(
            IPixelEventRepository eventRepository,
            IEventSummaryRepository summaryRepository,
            ILogger<MetricsCalculationService> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _summaryRepository = summaryRepository ?? throw new ArgumentNullException(nameof(summaryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FunnelMetrics> CalculateFunnelMetricsAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _eventRepository.GetByDateRangeAsync(startDate, endDate);
            var metrics = new FunnelMetrics
            {
                StartDate = startDate,
                EndDate = endDate
            };

            // Calculate basic metrics
            metrics.TotalVisits = events.Count(e => e.EventType == "visit");
            metrics.TotalRegistrations = events.Count(e => e.EventType == "registration");
            metrics.TotalDeposits = events.Count(e => e.EventType == "deposit");

            // Calculate rates
            metrics.VisitToRegistrationRate = CalculateRate(metrics.TotalRegistrations, metrics.TotalVisits);
            metrics.RegistrationToDepositRate = CalculateRate(metrics.TotalDeposits, metrics.TotalRegistrations);
            metrics.OverallConversionRate = CalculateRate(metrics.TotalDeposits, metrics.TotalVisits);

            // Calculate funnel steps
            metrics.Steps = CalculateFunnelSteps(events);

            // Calculate breakdowns
            metrics.Breakdowns = CalculateFunnelBreakdowns(events);

            return metrics;
        }

        public async Task<CohortMetrics> CalculateCohortMetricsAsync(DateTime cohortDate, int daysToAnalyze)
        {
            var endDate = cohortDate.AddDays(daysToAnalyze);
            var events = await _eventRepository.GetByDateRangeAsync(cohortDate, endDate);

            // Get cohort users (registered on cohort date)
            var cohortUsers = events
                .Where(e => e.EventType == "registration" && e.Timestamp.Date == cohortDate.Date)
                .Select(e => e.PlayerId)
                .ToList();

            var metrics = new CohortMetrics
            {
                CohortDate = cohortDate,
                CohortSize = cohortUsers.Count
            };

            // Calculate metrics for each period
            for (int day = 0; day <= daysToAnalyze; day++)
            {
                var currentDate = cohortDate.AddDays(day);
                var activeUsers = events
                    .Where(e => e.Timestamp.Date == currentDate.Date && cohortUsers.Contains(e.PlayerId))
                    .Select(e => e.PlayerId)
                    .Distinct()
                    .Count();

                metrics.Periods.Add(new CohortPeriod
                {
                    DayNumber = day,
                    ActiveUsers = activeUsers,
                    RetentionRate = CalculateRate(activeUsers, metrics.CohortSize),
                    AverageValue = CalculateAverageValue(events, cohortUsers, currentDate)
                });
            }

            // Calculate aggregate metrics
            metrics.Metrics["d1_retention"] = metrics.Periods.FirstOrDefault(p => p.DayNumber == 1)?.RetentionRate ?? 0;
            metrics.Metrics["d7_retention"] = metrics.Periods.FirstOrDefault(p => p.DayNumber == 7)?.RetentionRate ?? 0;
            metrics.Metrics["d30_retention"] = metrics.Periods.FirstOrDefault(p => p.DayNumber == 30)?.RetentionRate ?? 0;

            return metrics;
        }

        public async Task<RetentionMetrics> CalculateRetentionMetricsAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _eventRepository.GetByDateRangeAsync(startDate, endDate);
            var metrics = new RetentionMetrics
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var registeredUsers = events
                .Where(e => e.EventType == "registration")
                .GroupBy(e => e.PlayerId)
                .ToDictionary(g => g.Key, g => g.First().Timestamp.Date);

            // Calculate daily retention
            for (int day = 0; day <= (endDate - startDate).Days; day++)
            {
                var date = startDate.AddDays(day);
                var activeUsers = events
                    .Where(e => e.Timestamp.Date == date)
                    .Select(e => e.PlayerId)
                    .Distinct()
                    .Count();

                var eligibleUsers = registeredUsers
                    .Count(u => u.Value <= date);

                metrics.DailyRetention.Add(new RetentionDay
                {
                    DayNumber = day,
                    RetainedUsers = activeUsers,
                    RetentionRate = CalculateRate(activeUsers, eligibleUsers),
                    ChurnRate = 100 - CalculateRate(activeUsers, eligibleUsers)
                });
            }

            // Calculate key retention metrics
            metrics.DayOneRetention = CalculateNDayRetention(events, registeredUsers, 1);
            metrics.DaySevenRetention = CalculateNDayRetention(events, registeredUsers, 7);
            metrics.DayThirtyRetention = CalculateNDayRetention(events, registeredUsers, 30);

            // Calculate segment retention
            metrics.SegmentRetention = CalculateSegmentRetention(events, registeredUsers);

            return metrics;
        }

        public async Task<CampaignROIMetrics> CalculateCampaignROIAsync(string campaignId, DateTime startDate, DateTime endDate)
        {
            var events = await _eventRepository.GetByDateRangeAsync(startDate, endDate);
            var campaignEvents = events.Where(e => e.CampaignId == campaignId).ToList();

            var metrics = new CampaignROIMetrics
            {
                CampaignId = campaignId,
                StartDate = startDate,
                EndDate = endDate
            };

            // Calculate basic metrics
            var visits = campaignEvents.Count(e => e.EventType == "visit");
            var registrations = campaignEvents.Count(e => e.EventType == "registration");
            var deposits = campaignEvents.Count(e => e.EventType == "deposit");

            // Calculate costs and revenue (example values - in real implementation, get from external service)
            metrics.TotalCost = visits * 0.5m; // Example: $0.50 per click
            metrics.TotalRevenue = CalculateCampaignRevenue(campaignEvents);

            // Calculate ROI metrics
            metrics.ROI = CalculateROI(metrics.TotalRevenue, metrics.TotalCost);
            metrics.CostPerAcquisition = registrations > 0 ? metrics.TotalCost / registrations : 0;
            metrics.CustomerLifetimeValue = CalculateAverageLTV(campaignEvents);

            // Additional metrics
            metrics.Metrics["click_through_rate"] = CalculateRate(registrations, visits);
            metrics.Metrics["conversion_rate"] = CalculateRate(deposits, registrations);
            metrics.Metrics["revenue_per_visit"] = visits > 0 ? metrics.TotalRevenue / visits : 0;

            return metrics;
        }

        public async Task<PlayerLTVMetrics> CalculatePlayerLTVAsync(string playerId)
        {
            var events = await _eventRepository.GetByPlayerIdAsync(playerId);
            var registrationEvent = events.FirstOrDefault(e => e.EventType == "registration");

            if (registrationEvent == null)
                throw new InvalidOperationException($"No registration event found for player {playerId}");

            var metrics = new PlayerLTVMetrics
            {
                PlayerId = playerId,
                DaysSinceRegistration = (int)(DateTime.UtcNow - registrationEvent.Timestamp).TotalDays
            };

            // Calculate current value
            metrics.CurrentValue = CalculatePlayerValue(events);

            // Calculate predicted value using simple projection
            metrics.PredictedValue = PredictPlayerValue(events, metrics.CurrentValue);

            // Calculate churn probability
            metrics.ChurnProbability = CalculateChurnProbability(events);

            // Calculate value over time
            metrics.ValueOverTime = CalculateValueOverTime(events);

            // Calculate key indicators
            metrics.Indicators = CalculatePlayerIndicators(events);

            return metrics;
        }

        private Dictionary<string, FunnelStep> CalculateFunnelSteps(IEnumerable<PixelEvent> events)
        {
            var steps = new Dictionary<string, FunnelStep>();
            var orderedEvents = events.OrderBy(e => e.Timestamp).ToList();

            // Visit step
            var visits = orderedEvents.Where(e => e.EventType == "visit").ToList();
            steps["visit"] = new FunnelStep
            {
                Name = "Visit",
                Count = visits.Count,
                DropoffRate = 0,
                ConversionRate = 100,
                AverageTimeToNextStep = TimeSpan.Zero
            };

            // Registration step
            var registrations = orderedEvents.Where(e => e.EventType == "registration").ToList();
            steps["registration"] = new FunnelStep
            {
                Name = "Registration",
                Count = registrations.Count,
                DropoffRate = CalculateRate(visits.Count - registrations.Count, visits.Count),
                ConversionRate = CalculateRate(registrations.Count, visits.Count),
                AverageTimeToNextStep = CalculateAverageTimeBetweenSteps(visits, registrations)
            };

            // Deposit step
            var deposits = orderedEvents.Where(e => e.EventType == "deposit").ToList();
            steps["deposit"] = new FunnelStep
            {
                Name = "Deposit",
                Count = deposits.Count,
                DropoffRate = CalculateRate(registrations.Count - deposits.Count, registrations.Count),
                ConversionRate = CalculateRate(deposits.Count, registrations.Count),
                AverageTimeToNextStep = CalculateAverageTimeBetweenSteps(registrations, deposits)
            };

            return steps;
        }

        private List<FunnelBreakdown> CalculateFunnelBreakdowns(IEnumerable<PixelEvent> events)
        {
            var breakdowns = new List<FunnelBreakdown>();

            // Breakdown by campaign
            var campaignGroups = events.GroupBy(e => e.CampaignId);
            foreach (var group in campaignGroups)
            {
                var visits = group.Count(e => e.EventType == "visit");
                var deposits = group.Count(e => e.EventType == "deposit");

                breakdowns.Add(new FunnelBreakdown
                {
                    Dimension = "campaign",
                    Value = group.Key,
                    ConversionRate = CalculateRate(deposits, visits),
                    Volume = visits
                });
            }

            // Add more breakdowns (device, country, etc.) based on metadata

            return breakdowns;
        }

        private decimal CalculateAverageValue(IEnumerable<PixelEvent> events, List<string> cohortUsers, DateTime date)
        {
            var deposits = events
                .Where(e => e.EventType == "deposit" && 
                           e.Timestamp.Date == date.Date && 
                           cohortUsers.Contains(e.PlayerId))
                .ToList();

            if (!deposits.Any())
                return 0;

            var totalAmount = deposits.Sum(e => 
                decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0);

            return totalAmount / cohortUsers.Count;
        }

        private decimal CalculateNDayRetention(
            IEnumerable<PixelEvent> events,
            Dictionary<string, DateTime> registeredUsers,
            int days)
        {
            var retainedUsers = 0;
            var eligibleUsers = 0;

            foreach (var user in registeredUsers)
            {
                var targetDate = user.Value.AddDays(days);
                if (targetDate > DateTime.UtcNow)
                    continue;

                eligibleUsers++;

                if (events.Any(e => e.PlayerId == user.Key && e.Timestamp.Date == targetDate.Date))
                    retainedUsers++;
            }

            return CalculateRate(retainedUsers, eligibleUsers);
        }

        private Dictionary<string, decimal> CalculateSegmentRetention(
            IEnumerable<PixelEvent> events,
            Dictionary<string, DateTime> registeredUsers)
        {
            var segmentRetention = new Dictionary<string, decimal>();

            // Calculate retention by device type
            var deviceGroups = registeredUsers
                .GroupBy(u => events
                    .FirstOrDefault(e => e.PlayerId == u.Key && e.Metadata.ContainsKey("device_type"))
                    ?.Metadata["device_type"] ?? "unknown");

            foreach (var group in deviceGroups)
            {
                var retention = CalculateNDayRetention(events, 
                    group.ToDictionary(g => g.Key, g => g.Value), 7);
                segmentRetention[$"device_{group.Key}"] = retention;
            }

            return segmentRetention;
        }

        private decimal CalculateCampaignRevenue(IEnumerable<PixelEvent> events)
        {
            return events
                .Where(e => e.EventType == "deposit")
                .Sum(e => decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0);
        }

        private decimal CalculateROI(decimal revenue, decimal cost)
        {
            if (cost == 0)
                return 0;

            return ((revenue - cost) / cost) * 100;
        }

        private decimal CalculateAverageLTV(IEnumerable<PixelEvent> events)
        {
            var depositors = events
                .Where(e => e.EventType == "deposit")
                .GroupBy(e => e.PlayerId);

            if (!depositors.Any())
                return 0;

            var totalValue = depositors.Sum(g => g
                .Sum(e => decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0));

            return totalValue / depositors.Count();
        }

        private decimal CalculatePlayerValue(IEnumerable<PixelEvent> events)
        {
            return events
                .Where(e => e.EventType == "deposit")
                .Sum(e => decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0);
        }

        private decimal PredictPlayerValue(IEnumerable<PixelEvent> events, decimal currentValue)
        {
            // Simple linear projection based on current behavior
            var deposits = events.Where(e => e.EventType == "deposit").ToList();
            if (!deposits.Any())
                return currentValue;

            var depositFrequency = (DateTime.UtcNow - deposits.First().Timestamp).TotalDays / deposits.Count;
            var averageDepositAmount = currentValue / deposits.Count;
            var projectedDeposits = 365 / depositFrequency; // Project for one year

            return currentValue + (averageDepositAmount * (decimal)projectedDeposits);
        }

        private decimal CalculateChurnProbability(IEnumerable<PixelEvent> events)
        {
            var lastEvent = events.OrderByDescending(e => e.Timestamp).FirstOrDefault();
            if (lastEvent == null)
                return 100;

            var daysSinceLastActivity = (DateTime.UtcNow - lastEvent.Timestamp).TotalDays;

            // Simple churn model based on inactivity
            if (daysSinceLastActivity <= 7)
                return 0;
            if (daysSinceLastActivity <= 30)
                return 25;
            if (daysSinceLastActivity <= 90)
                return 75;
            return 100;
        }

        private List<ValuePeriod> CalculateValueOverTime(IEnumerable<PixelEvent> events)
        {
            var deposits = events
                .Where(e => e.EventType == "deposit")
                .OrderBy(e => e.Timestamp)
                .ToList();

            var periods = new List<ValuePeriod>();
            var cumulativeValue = 0m;

            foreach (var deposit in deposits)
            {
                if (decimal.TryParse(deposit.Metadata.GetValueOrDefault("amount"), out var amount))
                {
                    cumulativeValue += amount;
                    periods.Add(new ValuePeriod
                    {
                        Date = deposit.Timestamp,
                        Value = amount,
                        CumulativeValue = cumulativeValue
                    });
                }
            }

            return periods;
        }

        private Dictionary<string, decimal> CalculatePlayerIndicators(IEnumerable<PixelEvent> events)
        {
            var indicators = new Dictionary<string, decimal>();
            var deposits = events.Where(e => e.EventType == "deposit").ToList();

            // Average deposit amount
            if (deposits.Any())
            {
                var totalAmount = deposits.Sum(e => 
                    decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0);
                indicators["avg_deposit"] = totalAmount / deposits.Count;
            }

            // Deposit frequency (days between deposits)
            if (deposits.Count > 1)
            {
                var firstDeposit = deposits.Min(e => e.Timestamp);
                var lastDeposit = deposits.Max(e => e.Timestamp);
                var daysBetween = (lastDeposit - firstDeposit).TotalDays;
                indicators["deposit_frequency"] = (decimal)(daysBetween / (deposits.Count - 1));
            }

            return indicators;
        }

        private decimal CalculateRate(int numerator, int denominator)
        {
            if (denominator == 0)
                return 0;

            return (decimal)numerator / denominator * 100;
        }

        private TimeSpan CalculateAverageTimeBetweenSteps(List<PixelEvent> fromEvents, List<PixelEvent> toEvents)
        {
            if (!fromEvents.Any() || !toEvents.Any())
                return TimeSpan.Zero;

            var timeDiffs = new List<TimeSpan>();
            foreach (var toEvent in toEvents)
            {
                var fromEvent = fromEvents
                    .Where(e => e.PlayerId == toEvent.PlayerId && e.Timestamp <= toEvent.Timestamp)
                    .OrderByDescending(e => e.Timestamp)
                    .FirstOrDefault();

                if (fromEvent != null)
                {
                    timeDiffs.Add(toEvent.Timestamp - fromEvent.Timestamp);
                }
            }

            return timeDiffs.Any() 
                ? TimeSpan.FromTicks((long)timeDiffs.Average(t => t.Ticks))
                : TimeSpan.Zero;
        }
    }
}

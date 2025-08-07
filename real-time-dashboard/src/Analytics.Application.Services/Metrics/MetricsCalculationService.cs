using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Application.Models.Response;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Services.Metrics
{
    public class MetricsCalculationService : IMetricsCalculationService
    {
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly ILogger<MetricsCalculationService> _logger;

        public MetricsCalculationService(
            IEventSummaryRepository eventSummaryRepository,
            ILogger<MetricsCalculationService> logger)
        {
            _eventSummaryRepository = eventSummaryRepository ?? throw new ArgumentNullException(nameof(eventSummaryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ConversionMetricsResponse> CalculateConversionRatesAsync(DateTime startDate, DateTime endDate)
        {
            var summaries = await _eventSummaryRepository.GetByDateRangeAsync(startDate, endDate);
            var groupedByDate = summaries.GroupBy(s => s.EventDate);

            var dailyRates = new List<DailyConversionRate>();
            var campaignRates = new Dictionary<string, CampaignConversionRate>();

            int totalVisits = 0;
            int totalRegistrations = 0;
            int totalDeposits = 0;

            foreach (var group in groupedByDate)
            {
                var visits = group.Where(s => s.EventType == "visit").Sum(s => s.Count);
                var registrations = group.Where(s => s.EventType == "registration").Sum(s => s.Count);
                var deposits = group.Where(s => s.EventType == "deposit").Sum(s => s.Count);

                totalVisits += visits;
                totalRegistrations += registrations;
                totalDeposits += deposits;

                dailyRates.Add(new DailyConversionRate
                {
                    Date = group.Key,
                    VisitCount = visits,
                    RegistrationCount = registrations,
                    DepositCount = deposits,
                    ConversionRate = CalculateRate(registrations, visits),
                    DepositRate = CalculateRate(deposits, registrations)
                });

                // Calculate per-campaign rates
                foreach (var summary in group)
                {
                    if (!campaignRates.ContainsKey(summary.BannerTag))
                    {
                        campaignRates[summary.BannerTag] = new CampaignConversionRate
                        {
                            BannerTag = summary.BannerTag
                        };
                    }

                    var campaign = campaignRates[summary.BannerTag];
                    switch (summary.EventType)
                    {
                        case "visit":
                            campaign.VisitCount += summary.Count;
                            break;
                        case "registration":
                            campaign.RegistrationCount += summary.Count;
                            break;
                        case "deposit":
                            campaign.DepositCount += summary.Count;
                            break;
                    }
                }
            }

            // Calculate final campaign rates
            foreach (var campaign in campaignRates.Values)
            {
                campaign.ConversionRate = CalculateRate(campaign.RegistrationCount, campaign.VisitCount);
                campaign.DepositRate = CalculateRate(campaign.DepositCount, campaign.RegistrationCount);
            }

            return new ConversionMetricsResponse
            {
                OverallConversionRate = CalculateRate(totalRegistrations, totalVisits),
                DepositConversionRate = CalculateRate(totalDeposits, totalRegistrations),
                DailyRates = dailyRates.OrderBy(r => r.Date).ToList(),
                CampaignRates = campaignRates.Values.ToList()
            };
        }

        public async Task<CampaignPerformanceResponse> CalculateCampaignPerformanceAsync(string bannerTag, DateTime startDate, DateTime endDate)
        {
            var summaries = await _eventSummaryRepository.GetByDateRangeAsync(startDate, endDate);
            var campaignSummaries = summaries.Where(s => s.BannerTag == bannerTag);
            var otherCampaigns = summaries.Where(s => s.BannerTag != bannerTag);

            var dailyMetrics = new List<DailyPerformance>();
            var totalVisits = 0;
            var totalRegistrations = 0;
            var totalDeposits = 0;

            foreach (var date in EachDay(startDate, endDate))
            {
                var daySummaries = campaignSummaries.Where(s => s.EventDate == date);
                var visits = daySummaries.Where(s => s.EventType == "visit").Sum(s => s.Count);
                var registrations = daySummaries.Where(s => s.EventType == "registration").Sum(s => s.Count);
                var deposits = daySummaries.Where(s => s.EventType == "deposit").Sum(s => s.Count);

                totalVisits += visits;
                totalRegistrations += registrations;
                totalDeposits += deposits;

                dailyMetrics.Add(new DailyPerformance
                {
                    Date = date,
                    VisitCount = visits,
                    RegistrationCount = registrations,
                    DepositCount = deposits,
                    ConversionRate = CalculateRate(registrations, visits),
                    DepositRate = CalculateRate(deposits, registrations)
                });
            }

            // Calculate comparison metrics
            var campaignConversionRate = CalculateRate(totalRegistrations, totalVisits);
            var campaignDepositRate = CalculateRate(totalDeposits, totalRegistrations);

            var otherCampaignRates = otherCampaigns
                .GroupBy(s => s.BannerTag)
                .Select(g =>
                {
                    var visits = g.Where(s => s.EventType == "visit").Sum(s => s.Count);
                    var registrations = g.Where(s => s.EventType == "registration").Sum(s => s.Count);
                    var deposits = g.Where(s => s.EventType == "deposit").Sum(s => s.Count);
                    return new
                    {
                        ConversionRate = CalculateRate(registrations, visits),
                        DepositRate = CalculateRate(deposits, registrations)
                    };
                })
                .ToList();

            var avgConversionRate = otherCampaignRates.Average(r => r.ConversionRate);
            var avgDepositRate = otherCampaignRates.Average(r => r.DepositRate);

            var conversionPercentile = CalculatePercentile(campaignConversionRate, otherCampaignRates.Select(r => r.ConversionRate));
            var depositPercentile = CalculatePercentile(campaignDepositRate, otherCampaignRates.Select(r => r.DepositRate));

            return new CampaignPerformanceResponse
            {
                BannerTag = bannerTag,
                TotalVisits = totalVisits,
                TotalRegistrations = totalRegistrations,
                TotalDeposits = totalDeposits,
                ConversionRate = campaignConversionRate,
                DepositRate = campaignDepositRate,
                DailyMetrics = dailyMetrics,
                Comparison = new CampaignComparison
                {
                    AverageConversionRate = avgConversionRate,
                    ConversionRatePercentile = conversionPercentile,
                    AverageDepositRate = avgDepositRate,
                    DepositRatePercentile = depositPercentile,
                    Performance = DeterminePerformance(conversionPercentile)
                }
            };
        }

        public async Task<TrendMetricsResponse> CalculateTrendMetricsAsync(string eventType, DateTime startDate, DateTime endDate)
        {
            var summaries = await _eventSummaryRepository.GetByDateRangeAsync(startDate, endDate);
            var eventSummaries = summaries.Where(s => s.EventType == eventType)
                .OrderBy(s => s.EventDate)
                .ToList();

            var dataPoints = new List<TrendDataPoint>();
            int? previousCount = null;
            var movingAverages = new Queue<int>(7);
            var counts = new List<int>();

            foreach (var date in EachDay(startDate, endDate))
            {
                var count = eventSummaries
                    .Where(s => s.EventDate == date)
                    .Sum(s => s.Count);

                counts.Add(count);
                movingAverages.Enqueue(count);
                if (movingAverages.Count > 7)
                    movingAverages.Dequeue();

                dataPoints.Add(new TrendDataPoint
                {
                    Date = date,
                    Count = count,
                    ChangeFromPrevious = previousCount.HasValue ? CalculatePercentageChange(previousCount.Value, count) : 0,
                    MovingAverage = movingAverages.Average()
                });

                previousCount = count;
            }

            var firstWeekAvg = counts.Take(7).Average();
            var lastWeekAvg = counts.Skip(counts.Count - 7).Take(7).Average();
            var percentageChange = CalculatePercentageChange(firstWeekAvg, lastWeekAvg);

            // Analyze day-of-week patterns
            var dayOfWeekPatterns = counts
                .Zip(EachDay(startDate, endDate), (count, date) => new { count, date })
                .GroupBy(x => x.date.DayOfWeek)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(x => x.count)
                );

            return new TrendMetricsResponse
            {
                EventType = eventType,
                Trend = DetermineTrend(percentageChange, dataPoints),
                PercentageChange = percentageChange,
                DataPoints = dataPoints,
                MovingAverages = new MovingAverages
                {
                    SevenDay = CalculateMovingAverage(counts, 7),
                    ThirtyDay = CalculateMovingAverage(counts, 30),
                    NinetyDay = CalculateMovingAverage(counts, 90)
                },
                Seasonality = new SeasonalityPatterns
                {
                    HasDailyPattern = HasSignificantVariation(dayOfWeekPatterns.Values),
                    HasWeeklyPattern = HasWeeklyPattern(counts),
                    HasMonthlyPattern = HasMonthlyPattern(counts, startDate),
                    DayOfWeekPattern = dayOfWeekPatterns
                }
            };
        }

        private decimal CalculateRate(int numerator, int denominator)
        {
            if (denominator == 0) return 0;
            return (decimal)numerator / denominator * 100;
        }

        private decimal CalculatePercentageChange(double oldValue, double newValue)
        {
            if (oldValue == 0) return newValue > 0 ? 100 : 0;
            return (decimal)((newValue - oldValue) / oldValue * 100);
        }

        private decimal CalculatePercentile(decimal value, IEnumerable<decimal> values)
        {
            var sortedValues = values.OrderBy(v => v).ToList();
            var index = sortedValues.Count(v => v < value);
            return (decimal)index / sortedValues.Count * 100;
        }

        private string DeterminePerformance(decimal percentile)
        {
            if (percentile >= 75) return "Above Average";
            if (percentile >= 25) return "Average";
            return "Below Average";
        }

        private TrendDirection DetermineTrend(decimal percentageChange, List<TrendDataPoint> dataPoints)
        {
            var changes = dataPoints.Where(p => p.ChangeFromPrevious != 0).Select(p => p.ChangeFromPrevious).ToList();
            var volatility = changes.Any() ? changes.Average(Math.Abs) : 0;

            if (volatility > 50) return TrendDirection.Volatile;
            if (percentageChange > 10) return TrendDirection.Increasing;
            if (percentageChange < -10) return TrendDirection.Decreasing;
            return TrendDirection.Stable;
        }

        private decimal CalculateMovingAverage(List<int> values, int period)
        {
            if (values.Count < period) return values.Average();
            return values.Skip(values.Count - period).Average();
        }

        private bool HasSignificantVariation(IEnumerable<double> values)
        {
            var avg = values.Average();
            var variance = values.Average(v => Math.Pow(v - avg, 2));
            var stdDev = Math.Sqrt(variance);
            var coefficientOfVariation = stdDev / avg;
            return coefficientOfVariation > 0.2; // 20% variation threshold
        }

        private bool HasWeeklyPattern(List<int> values)
        {
            if (values.Count < 14) return false;
            var weeks = values.Count / 7;
            var weeklyTotals = new List<int>();

            for (int i = 0; i < weeks; i++)
            {
                weeklyTotals.Add(values.Skip(i * 7).Take(7).Sum());
            }

            return HasSignificantVariation(weeklyTotals.Select(v => (double)v));
        }

        private bool HasMonthlyPattern(List<int> values, DateTime startDate)
        {
            if (values.Count < 60) return false; // Need at least 2 months of data

            var monthlyTotals = values
                .Zip(EachDay(startDate, startDate.AddDays(values.Count - 1)), (count, date) => new { count, date })
                .GroupBy(x => new { x.date.Year, x.date.Month })
                .Select(g => g.Sum(x => x.count))
                .ToList();

            return HasSignificantVariation(monthlyTotals.Select(v => (double)v));
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }
    }
}

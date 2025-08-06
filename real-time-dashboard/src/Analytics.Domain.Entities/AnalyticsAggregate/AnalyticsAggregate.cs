using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;

namespace Analytics.Domain.Entities.AnalyticsAggregate
{
    /// <summary>
    /// Aggregate root for business intelligence and advanced analytics
    /// </summary>
    public class AnalyticsAggregate
    {
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPixelEventRepository _pixelEventRepository;

        public AnalyticsAggregate(
            IEventSummaryRepository eventSummaryRepository,
            IPlayerRepository playerRepository,
            IPixelEventRepository pixelEventRepository)
        {
            _eventSummaryRepository = eventSummaryRepository;
            _playerRepository = playerRepository;
            _pixelEventRepository = pixelEventRepository;
        }

        public async Task<FunnelAnalysis> AnalyzeFunnelAsync(DateTime fromDate, DateTime toDate, string bannerTag = null)
        {
            var summaries = await _eventSummaryRepository.GetByDateRangeAsync(fromDate, toDate);
            
            if (!string.IsNullOrEmpty(bannerTag))
            {
                summaries = summaries.Where(s => s.BannerTag == bannerTag);
            }

            var visits = summaries.Where(s => s.EventType == "visit").Sum(s => s.Count);
            var registrations = summaries.Where(s => s.EventType == "registration").Sum(s => s.Count);
            var deposits = summaries.Where(s => s.EventType == "deposit").Sum(s => s.Count);

            return new FunnelAnalysis
            {
                FromDate = fromDate,
                ToDate = toDate,
                BannerTag = bannerTag,
                Visits = visits,
                Registrations = registrations,
                Deposits = deposits,
                VisitToRegistrationRate = visits > 0 ? (double)registrations / visits : 0,
                RegistrationToDepositRate = registrations > 0 ? (double)deposits / registrations : 0,
                OverallConversionRate = visits > 0 ? (double)deposits / visits : 0
            };
        }

        public async Task<AnomalyDetection> DetectAnomaliesAsync(DateTime date, string bannerTag = null)
        {
            var currentDay = await GetDailyMetricsAsync(date, bannerTag);
            var previousDays = new List<DailyMetrics>();

            // Get metrics for the previous 7 days
            for (int i = 1; i <= 7; i++)
            {
                var previousDay = await GetDailyMetricsAsync(date.AddDays(-i), bannerTag);
                previousDays.Add(previousDay);
            }

            var avgVisits = previousDays.Average(d => d.TotalCount);
            var stdDevVisits = CalculateStandardDeviation(previousDays.Select(d => (double)d.TotalCount));

            var currentVisits = currentDay.TotalCount;
            var zScore = stdDevVisits > 0 ? Math.Abs(currentVisits - avgVisits) / stdDevVisits : 0;

            return new AnomalyDetection
            {
                Date = date,
                BannerTag = bannerTag,
                CurrentVisits = currentVisits,
                AverageVisits = avgVisits,
                StandardDeviation = stdDevVisits,
                ZScore = zScore,
                IsAnomaly = zScore > 2.0, // 2 standard deviations
                AnomalyType = currentVisits > avgVisits ? "spike" : "drop"
            };
        }

        public async Task<RetentionAnalysis> AnalyzeRetentionAsync(DateTime fromDate, DateTime toDate)
        {
            // This would require more complex player tracking
            // For now, return a basic structure
            return new RetentionAnalysis
            {
                FromDate = fromDate,
                ToDate = toDate,
                Day1Retention = 0.0,
                Day7Retention = 0.0,
                Day30Retention = 0.0,
                AverageRetentionRate = 0.0
            };
        }

        public async Task<GeographicAnalysis> AnalyzeGeographicDataAsync(DateTime fromDate, DateTime toDate, string bannerTag = null)
        {
            var events = await _pixelEventRepository.GetByDateRangeAsync(fromDate, toDate);
            
            if (!string.IsNullOrEmpty(bannerTag))
            {
                events = events.Where(e => e.BannerTag == bannerTag);
            }

            var geographicData = events
                .Where(e => !string.IsNullOrEmpty(e.SourceIp))
                .GroupBy(e => GetCountryFromIp(e.SourceIp))
                .Select(g => new GeographicMetrics
                {
                    Country = g.Key,
                    TotalEvents = g.Count(),
                    Visits = g.Count(e => e.EventType == "visit"),
                    Registrations = g.Count(e => e.EventType == "registration"),
                    Deposits = g.Count(e => e.EventType == "deposit")
                })
                .OrderByDescending(g => g.TotalEvents)
                .ToList();

            return new GeographicAnalysis
            {
                FromDate = fromDate,
                ToDate = toDate,
                BannerTag = bannerTag,
                GeographicMetrics = geographicData,
                TopCountries = geographicData.Take(10).ToList()
            };
        }

        private async Task<DailyMetrics> GetDailyMetricsAsync(DateTime date, string bannerTag = null)
        {
            var summaries = await _eventSummaryRepository.GetByDateRangeAsync(date, date);
            
            if (!string.IsNullOrEmpty(bannerTag))
            {
                summaries = summaries.Where(s => s.BannerTag == bannerTag);
            }

            return new DailyMetrics
            {
                Date = date,
                BannerTag = bannerTag,
                TotalCount = summaries.Sum(s => s.Count),
                EventBreakdown = summaries.GroupBy(s => s.EventType)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Count))
            };
        }

        private double CalculateStandardDeviation(IEnumerable<double> values)
        {
            var avg = values.Average();
            var variance = values.Select(x => Math.Pow(x - avg, 2)).Average();
            return Math.Sqrt(variance);
        }

        private string GetCountryFromIp(string ipAddress)
        {
            // This would typically use a GeoIP service
            // For now, return a placeholder
            return "Unknown";
        }
    }

    public class FunnelAnalysis
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string BannerTag { get; set; }
        public int Visits { get; set; }
        public int Registrations { get; set; }
        public int Deposits { get; set; }
        public double VisitToRegistrationRate { get; set; }
        public double RegistrationToDepositRate { get; set; }
        public double OverallConversionRate { get; set; }
    }

    public class AnomalyDetection
    {
        public DateTime Date { get; set; }
        public string BannerTag { get; set; }
        public int CurrentVisits { get; set; }
        public double AverageVisits { get; set; }
        public double StandardDeviation { get; set; }
        public double ZScore { get; set; }
        public bool IsAnomaly { get; set; }
        public string AnomalyType { get; set; }
    }

    public class RetentionAnalysis
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double Day1Retention { get; set; }
        public double Day7Retention { get; set; }
        public double Day30Retention { get; set; }
        public double AverageRetentionRate { get; set; }
    }

    public class GeographicAnalysis
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string BannerTag { get; set; }
        public List<GeographicMetrics> GeographicMetrics { get; set; } = new();
        public List<GeographicMetrics> TopCountries { get; set; } = new();
    }

    public class GeographicMetrics
    {
        public string Country { get; set; }
        public int TotalEvents { get; set; }
        public int Visits { get; set; }
        public int Registrations { get; set; }
        public int Deposits { get; set; }
    }
} 
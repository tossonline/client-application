using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;

namespace Analytics.Domain.Entities.CampaignAggregate
{
    /// <summary>
    /// Aggregate root for campaign/banner tag management and analytics
    /// </summary>
    public class CampaignAggregate
    {
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly IPixelEventRepository _pixelEventRepository;

        public CampaignAggregate(
            IEventSummaryRepository eventSummaryRepository,
            IPixelEventRepository pixelEventRepository)
        {
            _eventSummaryRepository = eventSummaryRepository;
            _pixelEventRepository = pixelEventRepository;
        }

        public async Task<CampaignPerformance> GetCampaignPerformanceAsync(string bannerTag, DateTime fromDate, DateTime toDate)
        {
            var summaries = await _eventSummaryRepository.GetByBannerTagAsync(bannerTag);
            
            var filteredSummaries = summaries.Where(s => s.EventDate >= fromDate && s.EventDate <= toDate).ToList();

            return new CampaignPerformance
            {
                BannerTag = bannerTag,
                FromDate = fromDate,
                ToDate = toDate,
                TotalVisits = filteredSummaries.Where(s => s.EventType == "visit").Sum(s => s.Count),
                TotalRegistrations = filteredSummaries.Where(s => s.EventType == "registration").Sum(s => s.Count),
                TotalDeposits = filteredSummaries.Where(s => s.EventType == "deposit").Sum(s => s.Count),
                DailyBreakdown = filteredSummaries.GroupBy(s => s.EventDate)
                    .ToDictionary(g => g.Key, g => new DailyCampaignMetrics
                    {
                        Date = g.Key,
                        Visits = g.Where(s => s.EventType == "visit").Sum(s => s.Count),
                        Registrations = g.Where(s => s.EventType == "registration").Sum(s => s.Count),
                        Deposits = g.Where(s => s.EventType == "deposit").Sum(s => s.Count)
                    })
            };
        }

        public async Task<List<CampaignComparison>> CompareCampaignsAsync(List<string> bannerTags, DateTime fromDate, DateTime toDate)
        {
            var comparisons = new List<CampaignComparison>();

            foreach (var bannerTag in bannerTags)
            {
                var performance = await GetCampaignPerformanceAsync(bannerTag, fromDate, toDate);
                
                comparisons.Add(new CampaignComparison
                {
                    BannerTag = bannerTag,
                    Performance = performance,
                    ConversionRate = performance.TotalVisits > 0 ? (double)performance.TotalDeposits / performance.TotalVisits : 0
                });
            }

            return comparisons.OrderByDescending(c => c.ConversionRate).ToList();
        }

        public async Task<CampaignTrends> GetCampaignTrendsAsync(string bannerTag, int days = 30)
        {
            var fromDate = DateTime.Today.AddDays(-days);
            var toDate = DateTime.Today;

            var performance = await GetCampaignPerformanceAsync(bannerTag, fromDate, toDate);

            return new CampaignTrends
            {
                BannerTag = bannerTag,
                FromDate = fromDate,
                ToDate = toDate,
                DailyMetrics = performance.DailyBreakdown.Values.ToList(),
                AverageDailyVisits = performance.DailyBreakdown.Values.Average(d => d.Visits),
                AverageDailyRegistrations = performance.DailyBreakdown.Values.Average(d => d.Registrations),
                AverageDailyDeposits = performance.DailyBreakdown.Values.Average(d => d.Deposits),
                TrendDirection = CalculateTrendDirection(performance.DailyBreakdown.Values.ToList())
            };
        }

        private string CalculateTrendDirection(List<DailyCampaignMetrics> dailyMetrics)
        {
            if (dailyMetrics.Count < 2) return "stable";

            var recentMetrics = dailyMetrics.TakeLast(7).ToList();
            var olderMetrics = dailyMetrics.SkipLast(7).TakeLast(7).ToList();

            if (recentMetrics.Count < 7 || olderMetrics.Count < 7) return "stable";

            var recentAvg = recentMetrics.Average(d => d.Visits);
            var olderAvg = olderMetrics.Average(d => d.Visits);

            if (recentAvg > olderAvg * 1.1) return "increasing";
            if (recentAvg < olderAvg * 0.9) return "decreasing";
            return "stable";
        }
    }

    public class CampaignPerformance
    {
        public string BannerTag { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalVisits { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalDeposits { get; set; }
        public Dictionary<DateTime, DailyCampaignMetrics> DailyBreakdown { get; set; } = new();
    }

    public class DailyCampaignMetrics
    {
        public DateTime Date { get; set; }
        public int Visits { get; set; }
        public int Registrations { get; set; }
        public int Deposits { get; set; }
    }

    public class CampaignComparison
    {
        public string BannerTag { get; set; }
        public CampaignPerformance Performance { get; set; }
        public double ConversionRate { get; set; }
    }

    public class CampaignTrends
    {
        public string BannerTag { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<DailyCampaignMetrics> DailyMetrics { get; set; } = new();
        public double AverageDailyVisits { get; set; }
        public double AverageDailyRegistrations { get; set; }
        public double AverageDailyDeposits { get; set; }
        public string TrendDirection { get; set; }
    }
} 
using System;
using System.Collections.Generic;

namespace Analytics.Application.Models.Response
{
    /// <summary>
    /// Response model for real-time metrics
    /// </summary>
    public class RealTimeMetricsResponse
    {
        /// <summary>
        /// Current timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Today's total visits
        /// </summary>
        public int TodayVisits { get; set; }

        /// <summary>
        /// Today's total registrations
        /// </summary>
        public int TodayRegistrations { get; set; }

        /// <summary>
        /// Today's total deposits
        /// </summary>
        public int TodayDeposits { get; set; }

        /// <summary>
        /// Current conversion rate
        /// </summary>
        public decimal CurrentConversionRate { get; set; }

        /// <summary>
        /// Current deposit rate
        /// </summary>
        public decimal CurrentDepositRate { get; set; }

        /// <summary>
        /// Hourly metrics for today
        /// </summary>
        public List<HourlyMetrics> HourlyBreakdown { get; set; } = new();

        /// <summary>
        /// Active campaigns
        /// </summary>
        public List<ActiveCampaign> ActiveCampaigns { get; set; } = new();

        /// <summary>
        /// Recent events
        /// </summary>
        public List<RecentEvent> RecentEvents { get; set; } = new();
    }

    public class HourlyMetrics
    {
        public int Hour { get; set; }
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
    }

    public class ActiveCampaign
    {
        public string BannerTag { get; set; }
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
        public string Performance { get; set; }
    }

    public class RecentEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string BannerTag { get; set; }
        public string PlayerId { get; set; }
    }
}

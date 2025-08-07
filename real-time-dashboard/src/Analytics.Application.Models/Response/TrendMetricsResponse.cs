using System;
using System.Collections.Generic;

namespace Analytics.Application.Models.Response
{
    /// <summary>
    /// Response model for trend analysis metrics
    /// </summary>
    public class TrendMetricsResponse
    {
        /// <summary>
        /// Event type being analyzed
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Overall trend direction
        /// </summary>
        public TrendDirection Trend { get; set; }

        /// <summary>
        /// Percentage change over the period
        /// </summary>
        public decimal PercentageChange { get; set; }

        /// <summary>
        /// Daily trend data points
        /// </summary>
        public List<TrendDataPoint> DataPoints { get; set; } = new();

        /// <summary>
        /// Moving averages for different periods
        /// </summary>
        public MovingAverages MovingAverages { get; set; }

        /// <summary>
        /// Seasonality patterns detected
        /// </summary>
        public SeasonalityPatterns Seasonality { get; set; }
    }

    public enum TrendDirection
    {
        Increasing,
        Decreasing,
        Stable,
        Volatile
    }

    public class TrendDataPoint
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal ChangeFromPrevious { get; set; }
        public decimal MovingAverage { get; set; }
    }

    public class MovingAverages
    {
        public decimal SevenDay { get; set; }
        public decimal ThirtyDay { get; set; }
        public decimal NinetyDay { get; set; }
    }

    public class SeasonalityPatterns
    {
        public bool HasDailyPattern { get; set; }
        public bool HasWeeklyPattern { get; set; }
        public bool HasMonthlyPattern { get; set; }
        public Dictionary<DayOfWeek, decimal> DayOfWeekPattern { get; set; }
        public Dictionary<int, decimal> DayOfMonthPattern { get; set; }
    }
}

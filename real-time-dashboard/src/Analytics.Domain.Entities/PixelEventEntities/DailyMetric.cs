using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents daily metrics for analytics.
    /// This entity stores aggregated daily metrics including visit counts, registrations,
    /// deposits, conversion rates, and trend indicators.
    /// </summary>
    /// <remarks>
    /// Daily metrics provide a high-level view of performance on a daily basis,
    /// enabling trend analysis and performance monitoring. They are automatically
    /// calculated from event summaries and provide key performance indicators (KPIs)
    /// for business intelligence.
    /// </remarks>
    public class DailyMetric
    {
        /// <summary>
        /// Gets the unique identifier for the daily metric
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the date for which these metrics are calculated
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the type of event these metrics are for
        /// </summary>
        public string EventType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the total number of visits for the day
        /// </summary>
        public int VisitCount { get; private set; }

        /// <summary>
        /// Gets the total number of registrations for the day
        /// </summary>
        public int RegistrationCount { get; private set; }

        /// <summary>
        /// Gets the total number of deposits for the day
        /// </summary>
        public int DepositCount { get; private set; }

        /// <summary>
        /// Gets the conversion rate (registrations/visits) as a percentage
        /// </summary>
        public decimal ConversionRate { get; private set; }

        /// <summary>
        /// Gets the deposit rate (deposits/registrations) as a percentage
        /// </summary>
        public decimal DepositRate { get; private set; }

        /// <summary>
        /// Gets the trend indicator comparing to the previous day
        /// </summary>
        public TrendIndicator Trend { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private DailyMetric() { }

        /// <summary>
        /// Creates a new daily metric
        /// </summary>
        /// <param name="date">The date for the metrics</param>
        /// <param name="eventType">The type of event to track</param>
        /// <returns>A new DailyMetric instance</returns>
        /// <exception cref="ArgumentException">Thrown when eventType is null or empty</exception>
        public static DailyMetric Create(DateTime date, string eventType)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));

            return new DailyMetric
            {
                Date = date.Date,
                EventType = eventType,
                VisitCount = 0,
                RegistrationCount = 0,
                DepositCount = 0,
                ConversionRate = 0,
                DepositRate = 0,
                Trend = TrendIndicator.Stable
            };
        }

        /// <summary>
        /// Updates the visit count and recalculates rates
        /// </summary>
        /// <param name="count">The new visit count</param>
        /// <exception cref="ArgumentException">Thrown when count is negative</exception>
        /// <remarks>
        /// This method updates the visit count and automatically recalculates
        /// the conversion rate based on the new value
        /// </remarks>
        public void UpdateVisitCount(int count)
        {
            if (count < 0)
                throw new ArgumentException("Count cannot be negative", nameof(count));

            VisitCount = count;
            RecalculateRates();
        }

        /// <summary>
        /// Updates the registration count and recalculates rates
        /// </summary>
        /// <param name="count">The new registration count</param>
        /// <exception cref="ArgumentException">Thrown when count is negative</exception>
        /// <remarks>
        /// This method updates the registration count and automatically recalculates
        /// both the conversion rate and deposit rate based on the new value
        /// </remarks>
        public void UpdateRegistrationCount(int count)
        {
            if (count < 0)
                throw new ArgumentException("Count cannot be negative", nameof(count));

            RegistrationCount = count;
            RecalculateRates();
        }

        /// <summary>
        /// Updates the deposit count and recalculates rates
        /// </summary>
        /// <param name="count">The new deposit count</param>
        /// <exception cref="ArgumentException">Thrown when count is negative</exception>
        /// <remarks>
        /// This method updates the deposit count and automatically recalculates
        /// the deposit rate based on the new value
        /// </remarks>
        public void UpdateDepositCount(int count)
        {
            if (count < 0)
                throw new ArgumentException("Count cannot be negative", nameof(count));

            DepositCount = count;
            RecalculateRates();
        }

        /// <summary>
        /// Updates the trend based on comparison with the previous day's metrics
        /// </summary>
        /// <param name="previousDay">The previous day's metrics</param>
        /// <remarks>
        /// The trend is determined by comparing the conversion rates:
        /// - Increasing: Current rate is more than 1% higher
        /// - Decreasing: Current rate is more than 1% lower
        /// - Stable: Difference is within ±1%
        /// </remarks>
        public void UpdateTrend(DailyMetric previousDay)
        {
            if (previousDay == null)
            {
                Trend = TrendIndicator.Stable;
                return;
            }

            var conversionDiff = ConversionRate - previousDay.ConversionRate;
            Trend = conversionDiff switch
            {
                > 1 => TrendIndicator.Increasing,
                < -1 => TrendIndicator.Decreasing,
                _ => TrendIndicator.Stable
            };
        }

        /// <summary>
        /// Recalculates conversion and deposit rates based on current counts
        /// </summary>
        /// <remarks>
        /// - Conversion Rate = (Registrations / Visits) * 100
        /// - Deposit Rate = (Deposits / Registrations) * 100
        /// Rates are set to 0 if the denominator is 0
        /// </remarks>
        private void RecalculateRates()
        {
            ConversionRate = VisitCount > 0 
                ? (decimal)RegistrationCount / VisitCount * 100 
                : 0;

            DepositRate = RegistrationCount > 0 
                ? (decimal)DepositCount / RegistrationCount * 100 
                : 0;
        }
    }

    /// <summary>
    /// Represents the trend direction for metrics
    /// </summary>
    public enum TrendIndicator
    {
        /// <summary>
        /// Metrics are improving compared to the previous period
        /// </summary>
        Increasing,

        /// <summary>
        /// Metrics are relatively unchanged from the previous period
        /// </summary>
        Stable,

        /// <summary>
        /// Metrics are declining compared to the previous period
        /// </summary>
        Decreasing
    }
}
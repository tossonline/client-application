using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Services.Players
{
    /// <summary>
    /// Service interface for player segmentation
    /// </summary>
    public interface IPlayerSegmentationService
    {
        /// <summary>
        /// Calculate player segment
        /// </summary>
        Task<PlayerSegmentInfo> CalculateSegmentAsync(string playerId);

        /// <summary>
        /// Calculate segments for multiple players
        /// </summary>
        Task<IEnumerable<PlayerSegmentInfo>> CalculateSegmentsAsync(IEnumerable<string> playerIds);

        /// <summary>
        /// Get segment distribution
        /// </summary>
        Task<SegmentDistribution> GetSegmentDistributionAsync();

        /// <summary>
        /// Get segment transition metrics
        /// </summary>
        Task<SegmentTransitionMetrics> GetSegmentTransitionsAsync(DateTime startDate, DateTime endDate);
    }

    public class PlayerSegmentInfo
    {
        public string PlayerId { get; set; }
        public PlayerSegment CurrentSegment { get; set; }
        public PlayerSegment PreviousSegment { get; set; }
        public DateTime LastTransition { get; set; }
        public Dictionary<string, decimal> Metrics { get; set; } = new();
        public List<SegmentRule> MatchedRules { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    public class SegmentRule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMatched { get; set; }
        public decimal? Threshold { get; set; }
        public decimal? ActualValue { get; set; }
    }

    public class SegmentDistribution
    {
        public DateTime CalculatedAt { get; set; }
        public Dictionary<PlayerSegment, int> Distribution { get; set; } = new();
        public Dictionary<PlayerSegment, decimal> Percentages { get; set; } = new();
        public Dictionary<PlayerSegment, SegmentMetrics> Metrics { get; set; } = new();
    }

    public class SegmentMetrics
    {
        public int PlayerCount { get; set; }
        public decimal AverageValue { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal ChurnRate { get; set; }
        public Dictionary<string, decimal> CustomMetrics { get; set; } = new();
    }

    public class SegmentTransitionMetrics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<PlayerSegment, Dictionary<PlayerSegment, int>> TransitionMatrix { get; set; } = new();
        public Dictionary<(PlayerSegment From, PlayerSegment To), decimal> TransitionRates { get; set; } = new();
        public List<SegmentTransition> TopTransitions { get; set; } = new();
    }

    public class SegmentTransition
    {
        public PlayerSegment FromSegment { get; set; }
        public PlayerSegment ToSegment { get; set; }
        public int Count { get; set; }
        public decimal Rate { get; set; }
        public List<string> Factors { get; set; } = new();
    }
}

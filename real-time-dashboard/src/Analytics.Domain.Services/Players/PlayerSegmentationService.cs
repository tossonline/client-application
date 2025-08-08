using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Domain.Services.Players
{
    public class PlayerSegmentationService : IPlayerSegmentationService
    {
        private readonly IPixelEventRepository _eventRepository;
        private readonly ILogger<PlayerSegmentationService> _logger;

        private readonly Dictionary<PlayerSegment, List<Func<Player, Task<SegmentRule>>>> _segmentRules;

        public PlayerSegmentationService(
            IPixelEventRepository eventRepository,
            ILogger<PlayerSegmentationService> logger)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _segmentRules = new Dictionary<PlayerSegment, List<Func<Player, Task<SegmentRule>>>>
            {
                [PlayerSegment.VIP] = new()
                {
                    HighValueRule,
                    FrequentDepositRule,
                    LongTermPlayerRule
                },
                [PlayerSegment.Regular] = new()
                {
                    RegularDepositRule,
                    ConsistentActivityRule
                },
                [PlayerSegment.NonDepositor] = new()
                {
                    NoDepositsRule,
                    LongRegistrationTimeRule
                },
                [PlayerSegment.Inactive] = new()
                {
                    InactivityRule,
                    ChurnRiskRule
                }
            };
        }

        public async Task<PlayerSegmentInfo> CalculateSegmentAsync(string playerId)
        {
            var events = await _eventRepository.GetByPlayerIdAsync(playerId);
            var player = BuildPlayerFromEvents(events);

            var segmentInfo = new PlayerSegmentInfo
            {
                PlayerId = playerId,
                PreviousSegment = player.Segment,
                CurrentSegment = PlayerSegment.New // Default segment
            };

            // Calculate metrics
            segmentInfo.Metrics = await CalculatePlayerMetrics(player, events);

            // Evaluate segment rules
            var matchedRules = new List<SegmentRule>();
            foreach (var segmentRules in _segmentRules)
            {
                var rules = await Task.WhenAll(segmentRules.Value.Select(rule => rule(player)));
                var matchedSegmentRules = rules.Where(r => r.IsMatched).ToList();
                
                if (matchedSegmentRules.Count == segmentRules.Value.Count)
                {
                    segmentInfo.CurrentSegment = segmentRules.Key;
                    matchedRules.AddRange(matchedSegmentRules);
                    break;
                }
            }

            segmentInfo.MatchedRules = matchedRules;
            segmentInfo.LastTransition = DateTime.UtcNow;

            // Generate recommendations
            segmentInfo.Recommendations = GenerateRecommendations(player, segmentInfo);

            return segmentInfo;
        }

        public async Task<IEnumerable<PlayerSegmentInfo>> CalculateSegmentsAsync(IEnumerable<string> playerIds)
        {
            var tasks = playerIds.Select(CalculateSegmentAsync);
            return await Task.WhenAll(tasks);
        }

        public async Task<SegmentDistribution> GetSegmentDistributionAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            var players = events
                .GroupBy(e => e.PlayerId)
                .Select(g => BuildPlayerFromEvents(g))
                .ToList();

            var distribution = new SegmentDistribution
            {
                CalculatedAt = DateTime.UtcNow
            };

            // Calculate distribution
            foreach (var segment in Enum.GetValues<PlayerSegment>())
            {
                var segmentPlayers = players.Where(p => p.Segment == segment).ToList();
                distribution.Distribution[segment] = segmentPlayers.Count;
                distribution.Percentages[segment] = players.Any() 
                    ? (decimal)segmentPlayers.Count / players.Count * 100 
                    : 0;

                // Calculate segment metrics
                distribution.Metrics[segment] = await CalculateSegmentMetrics(segmentPlayers, events);
            }

            return distribution;
        }

        public async Task<SegmentTransitionMetrics> GetSegmentTransitionsAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _eventRepository.GetByDateRangeAsync(startDate, endDate);
            var playerSegments = await TrackSegmentTransitions(events);

            var metrics = new SegmentTransitionMetrics
            {
                StartDate = startDate,
                EndDate = endDate,
                TransitionMatrix = new Dictionary<PlayerSegment, Dictionary<PlayerSegment, int>>(),
                TransitionRates = new Dictionary<(PlayerSegment From, PlayerSegment To), decimal>()
            };

            // Build transition matrix
            foreach (var segment in Enum.GetValues<PlayerSegment>())
            {
                metrics.TransitionMatrix[segment] = new Dictionary<PlayerSegment, int>();
                foreach (var toSegment in Enum.GetValues<PlayerSegment>())
                {
                    metrics.TransitionMatrix[segment][toSegment] = 0;
                }
            }

            // Calculate transitions
            foreach (var player in playerSegments)
            {
                var transitions = player.Value
                    .Zip(player.Value.Skip(1), (a, b) => (From: a.Segment, To: b.Segment))
                    .ToList();

                foreach (var transition in transitions)
                {
                    metrics.TransitionMatrix[transition.From][transition.To]++;
                }
            }

            // Calculate transition rates
            foreach (var fromSegment in metrics.TransitionMatrix.Keys)
            {
                var totalTransitions = metrics.TransitionMatrix[fromSegment].Values.Sum();
                foreach (var toSegment in metrics.TransitionMatrix[fromSegment].Keys)
                {
                    var rate = totalTransitions > 0
                        ? (decimal)metrics.TransitionMatrix[fromSegment][toSegment] / totalTransitions * 100
                        : 0;
                    metrics.TransitionRates[(fromSegment, toSegment)] = rate;
                }
            }

            // Find top transitions
            metrics.TopTransitions = metrics.TransitionRates
                .Where(t => t.Value > 0)
                .OrderByDescending(t => t.Value)
                .Take(5)
                .Select(t => new SegmentTransition
                {
                    FromSegment = t.Key.From,
                    ToSegment = t.Key.To,
                    Count = metrics.TransitionMatrix[t.Key.From][t.Key.To],
                    Rate = t.Value,
                    Factors = IdentifyTransitionFactors(t.Key.From, t.Key.To)
                })
                .ToList();

            return metrics;
        }

        private Player BuildPlayerFromEvents(IEnumerable<PixelEvent> events)
        {
            var firstEvent = events.OrderBy(e => e.Timestamp).First();
            var player = Player.Create(firstEvent.PlayerId);

            foreach (var e in events.OrderBy(e => e.Timestamp))
            {
                player.UpdateLastEvent(e.Timestamp);

                switch (e.EventType)
                {
                    case "registration":
                        player.Register();
                        break;
                    case "deposit":
                        if (decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount))
                        {
                            player.Deposit(amount);
                        }
                        break;
                }
            }

            return player;
        }

        private async Task<Dictionary<string, decimal>> CalculatePlayerMetrics(Player player, IEnumerable<PixelEvent> events)
        {
            var metrics = new Dictionary<string, decimal>();

            // Activity metrics
            metrics["days_since_registration"] = player.RegistrationDate.HasValue
                ? (decimal)(DateTime.UtcNow - player.RegistrationDate.Value).TotalDays
                : 0;

            metrics["days_since_last_activity"] = player.LastEventAt.HasValue
                ? (decimal)(DateTime.UtcNow - player.LastEventAt.Value).TotalDays
                : 0;

            // Value metrics
            var deposits = events.Where(e => e.EventType == "deposit").ToList();
            metrics["total_deposits"] = deposits.Count;
            metrics["total_deposit_amount"] = deposits.Sum(e =>
                decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0);

            if (deposits.Any())
            {
                metrics["average_deposit_amount"] = metrics["total_deposit_amount"] / metrics["total_deposits"];
                metrics["deposit_frequency"] = metrics["days_since_registration"] / metrics["total_deposits"];
            }

            // Engagement metrics
            var visits = events.Count(e => e.EventType == "visit");
            metrics["total_visits"] = visits;
            metrics["visits_per_day"] = metrics["days_since_registration"] > 0
                ? visits / metrics["days_since_registration"]
                : 0;

            return metrics;
        }

        private async Task<SegmentMetrics> CalculateSegmentMetrics(List<Player> players, IEnumerable<PixelEvent> events)
        {
            if (!players.Any())
                return new SegmentMetrics();

            var metrics = new SegmentMetrics
            {
                PlayerCount = players.Count
            };

            // Calculate average value
            var playerEvents = events.Where(e => players.Select(p => p.PlayerId).Contains(e.PlayerId));
            var deposits = playerEvents.Where(e => e.EventType == "deposit");
            var totalValue = deposits.Sum(e =>
                decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount) ? amount : 0);
            metrics.AverageValue = totalValue / players.Count;

            // Calculate retention
            var activeInLast30Days = players.Count(p => 
                p.LastEventAt.HasValue && (DateTime.UtcNow - p.LastEventAt.Value).TotalDays <= 30);
            metrics.RetentionRate = (decimal)activeInLast30Days / players.Count * 100;
            metrics.ChurnRate = 100 - metrics.RetentionRate;

            // Add custom metrics
            metrics.CustomMetrics["avg_deposits_per_month"] = deposits.Count() / (decimal)players.Count;
            metrics.CustomMetrics["avg_lifetime_days"] = players.Average(p => 
                (DateTime.UtcNow - p.FirstSeen).TotalDays);

            return metrics;
        }

        private async Task<Dictionary<string, List<(DateTime Timestamp, PlayerSegment Segment)>>> TrackSegmentTransitions(
            IEnumerable<PixelEvent> events)
        {
            var transitions = new Dictionary<string, List<(DateTime, PlayerSegment)>>();

            foreach (var playerEvents in events.GroupBy(e => e.PlayerId))
            {
                var segmentHistory = new List<(DateTime, PlayerSegment)>();
                var player = Player.Create(playerEvents.Key);
                segmentHistory.Add((player.FirstSeen, player.Segment));

                foreach (var e in playerEvents.OrderBy(e => e.Timestamp))
                {
                    player.UpdateLastEvent(e.Timestamp);

                    switch (e.EventType)
                    {
                        case "registration":
                            player.Register();
                            segmentHistory.Add((e.Timestamp, player.Segment));
                            break;
                        case "deposit":
                            if (decimal.TryParse(e.Metadata.GetValueOrDefault("amount"), out var amount))
                            {
                                player.Deposit(amount);
                                segmentHistory.Add((e.Timestamp, player.Segment));
                            }
                            break;
                    }
                }

                transitions[playerEvents.Key] = segmentHistory;
            }

            return transitions;
        }

        private List<string> IdentifyTransitionFactors(PlayerSegment from, PlayerSegment to)
        {
            var factors = new List<string>();

            switch ((from, to))
            {
                case (PlayerSegment.New, PlayerSegment.Regular):
                    factors.Add("First deposit made");
                    factors.Add("Regular activity pattern established");
                    break;
                case (PlayerSegment.Regular, PlayerSegment.VIP):
                    factors.Add("Increased deposit frequency");
                    factors.Add("Higher deposit amounts");
                    factors.Add("Consistent long-term activity");
                    break;
                case (PlayerSegment.Regular, PlayerSegment.Inactive):
                    factors.Add("Extended period of inactivity");
                    factors.Add("Decreased deposit frequency");
                    break;
                case (PlayerSegment.NonDepositor, PlayerSegment.Regular):
                    factors.Add("First deposit after long registration period");
                    factors.Add("Increased engagement");
                    break;
                // Add more transition factors as needed
            }

            return factors;
        }

        private List<string> GenerateRecommendations(Player player, PlayerSegmentInfo segmentInfo)
        {
            var recommendations = new List<string>();

            switch (segmentInfo.CurrentSegment)
            {
                case PlayerSegment.New:
                    recommendations.Add("Send welcome message and onboarding guide");
                    recommendations.Add("Offer first deposit bonus");
                    break;

                case PlayerSegment.Regular:
                    if (segmentInfo.Metrics["deposit_frequency"] > 7)
                        recommendations.Add("Offer loyalty rewards for increased deposit frequency");
                    if (segmentInfo.Metrics["average_deposit_amount"] < 100)
                        recommendations.Add("Present higher-value deposit options");
                    break;

                case PlayerSegment.VIP:
                    recommendations.Add("Provide personalized VIP manager contact");
                    recommendations.Add("Offer exclusive promotions and events");
                    recommendations.Add("Consider for VIP loyalty program");
                    break;

                case PlayerSegment.NonDepositor:
                    var daysRegistered = segmentInfo.Metrics["days_since_registration"];
                    if (daysRegistered <= 7)
                        recommendations.Add("Send first deposit reminder with bonus offer");
                    else if (daysRegistered <= 30)
                        recommendations.Add("Offer special deposit promotion for new players");
                    else
                        recommendations.Add("Re-engagement campaign with deposit incentives");
                    break;

                case PlayerSegment.Inactive:
                    var daysSinceActivity = segmentInfo.Metrics["days_since_last_activity"];
                    if (daysSinceActivity <= 30)
                        recommendations.Add("Send re-engagement email with personalized offer");
                    else if (daysSinceActivity <= 90)
                        recommendations.Add("Offer special 'welcome back' bonus");
                    else
                        recommendations.Add("Add to win-back campaign");
                    break;
            }

            return recommendations;
        }

        #region Segment Rules

        private async Task<SegmentRule> HighValueRule(Player player)
        {
            const decimal threshold = 1000;
            return new SegmentRule
            {
                Name = "High Value",
                Description = $"Total deposits exceed {threshold:C}",
                Threshold = threshold,
                ActualValue = player.TotalDepositAmount,
                IsMatched = player.TotalDepositAmount >= threshold
            };
        }

        private async Task<SegmentRule> FrequentDepositRule(Player player)
        {
            const int threshold = 5;
            return new SegmentRule
            {
                Name = "Frequent Deposits",
                Description = $"Made at least {threshold} deposits",
                Threshold = threshold,
                ActualValue = player.TotalDeposits,
                IsMatched = player.TotalDeposits >= threshold
            };
        }

        private async Task<SegmentRule> LongTermPlayerRule(Player player)
        {
            const int threshold = 90;
            var daysActive = player.GetLifetimeDays();
            return new SegmentRule
            {
                Name = "Long Term Player",
                Description = $"Active for at least {threshold} days",
                Threshold = threshold,
                ActualValue = daysActive,
                IsMatched = daysActive >= threshold
            };
        }

        private async Task<SegmentRule> RegularDepositRule(Player player)
        {
            const int threshold = 2;
            return new SegmentRule
            {
                Name = "Regular Deposits",
                Description = $"Made at least {threshold} deposits",
                Threshold = threshold,
                ActualValue = player.TotalDeposits,
                IsMatched = player.TotalDeposits >= threshold
            };
        }

        private async Task<SegmentRule> ConsistentActivityRule(Player player)
        {
            const int threshold = 7;
            var daysSinceActivity = player.LastEventAt.HasValue
                ? (DateTime.UtcNow - player.LastEventAt.Value).TotalDays
                : double.MaxValue;
            return new SegmentRule
            {
                Name = "Consistent Activity",
                Description = $"Active within last {threshold} days",
                Threshold = threshold,
                ActualValue = (decimal)daysSinceActivity,
                IsMatched = daysSinceActivity <= threshold
            };
        }

        private async Task<SegmentRule> NoDepositsRule(Player player)
        {
            return new SegmentRule
            {
                Name = "No Deposits",
                Description = "Has not made any deposits",
                Threshold = 0,
                ActualValue = player.TotalDeposits,
                IsMatched = player.TotalDeposits == 0
            };
        }

        private async Task<SegmentRule> LongRegistrationTimeRule(Player player)
        {
            const int threshold = 7;
            var daysRegistered = player.RegistrationDate.HasValue
                ? (DateTime.UtcNow - player.RegistrationDate.Value).TotalDays
                : 0;
            return new SegmentRule
            {
                Name = "Long Registration Time",
                Description = $"Registered for at least {threshold} days",
                Threshold = threshold,
                ActualValue = (decimal)daysRegistered,
                IsMatched = daysRegistered >= threshold
            };
        }

        private async Task<SegmentRule> InactivityRule(Player player)
        {
            const int threshold = 30;
            var daysSinceActivity = player.LastEventAt.HasValue
                ? (DateTime.UtcNow - player.LastEventAt.Value).TotalDays
                : double.MaxValue;
            return new SegmentRule
            {
                Name = "Inactivity",
                Description = $"Inactive for at least {threshold} days",
                Threshold = threshold,
                ActualValue = (decimal)daysSinceActivity,
                IsMatched = daysSinceActivity >= threshold
            };
        }

        private async Task<SegmentRule> ChurnRiskRule(Player player)
        {
            const int threshold = 90;
            var daysSinceActivity = player.LastEventAt.HasValue
                ? (DateTime.UtcNow - player.LastEventAt.Value).TotalDays
                : double.MaxValue;
            return new SegmentRule
            {
                Name = "Churn Risk",
                Description = $"No activity for {threshold} days",
                Threshold = threshold,
                ActualValue = (decimal)daysSinceActivity,
                IsMatched = daysSinceActivity >= threshold
            };
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Analytics.Domain.Abstractions;
using Analytics.Domain.Entities.Common;
using Analytics.Domain.Events;
using System.Linq;

namespace Analytics.Domain.Entities.PixelEventAggregate.Entities
{
    /// <summary>
    /// Aggregate root for pixel events
    /// </summary>
    public class PixelEventAggregate : AggregateRoot
    {
        private readonly List<PixelEvent> _pixelEvents = new();

        /// <summary>
        /// Gets the unique identifier for the aggregate
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the player identifier
        /// </summary>
        public string PlayerId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the banner tag associated with this aggregate
        /// </summary>
        public BannerTag BannerTag { get; private set; } = null!;

        /// <summary>
        /// Gets the campaign identifier
        /// </summary>
        public string CampaignId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the collection of pixel events
        /// </summary>
        public IReadOnlyCollection<PixelEvent> PixelEvents => _pixelEvents.AsReadOnly();

        /// <summary>
        /// Gets the total number of events
        /// </summary>
        public int TotalEvents => _pixelEvents.Count;

        /// <summary>
        /// Gets the first event timestamp
        /// </summary>
        public DateTime? FirstEventAt { get; private set; }

        /// <summary>
        /// Gets the last event timestamp
        /// </summary>
        public DateTime? LastEventAt { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private PixelEventAggregate() { }

        /// <summary>
        /// Creates a new pixel event aggregate
        /// </summary>
        /// <param name="playerId">The player identifier</param>
        /// <param name="bannerTag">The banner tag</param>
        /// <returns>A new PixelEventAggregate instance</returns>
        public static PixelEventAggregate Create(string playerId, BannerTag bannerTag)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                throw new ArgumentException("Player ID cannot be null or empty", nameof(playerId));

            if (bannerTag == null)
                throw new ArgumentNullException(nameof(bannerTag));

            var aggregate = new PixelEventAggregate
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                BannerTag = bannerTag,
                CampaignId = bannerTag.CampaignId
            };

            return aggregate;
        }

        /// <summary>
        /// Adds a pixel event to the aggregate
        /// </summary>
        /// <param name="eventType">The type of event</param>
        /// <param name="sourceIp">Optional source IP</param>
        /// <param name="userAgent">Optional user agent</param>
        /// <param name="metadata">Optional metadata</param>
        /// <param name="timestamp">Optional timestamp (defaults to UTC now)</param>
        /// <returns>The created pixel event</returns>
        public PixelEvent AddEvent(
            EventType eventType,
            string? sourceIp = null,
            string? userAgent = null,
            Dictionary<string, string>? metadata = null,
            DateTime? timestamp = null)
        {
            if (eventType == null)
                throw new ArgumentNullException(nameof(eventType));

            var pixelEvent = new PixelEvent
            {
                EventType = eventType.Value,
                PlayerId = PlayerId,
                BannerTag = BannerTag.Value,
                SourceIp = sourceIp,
                UserAgent = userAgent,
                Timestamp = timestamp ?? DateTime.UtcNow
            };

            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    pixelEvent.AddMetadata(kvp.Key, kvp.Value);
                }
            }

            _pixelEvents.Add(pixelEvent);

            // Update aggregate state
            if (!FirstEventAt.HasValue)
                FirstEventAt = pixelEvent.Timestamp;

            LastEventAt = pixelEvent.Timestamp;

            // Raise domain events based on event type
            RaiseDomainEvent(new PixelEventReceived(
                pixelEvent.EventType,
                pixelEvent.PlayerId,
                pixelEvent.BannerTag,
                pixelEvent.Metadata,
                pixelEvent.SourceIp,
                pixelEvent.UserAgent,
                pixelEvent.Timestamp));

            if (eventType == EventType.Registration)
            {
                RaiseDomainEvent(new PlayerRegistered(
                    PlayerId,
                    BannerTag.Value,
                    pixelEvent.Timestamp,
                    sourceIp,
                    userAgent));
            }
            else if (eventType == EventType.Deposit)
            {
                var amount = 0m;
                if (metadata?.ContainsKey("amount") == true && decimal.TryParse(metadata["amount"], out var parsedAmount))
                {
                    amount = parsedAmount;
                }

                var isFirstDeposit = !_pixelEvents.Any(e => e.EventType == EventType.Deposit.Value);
                RaiseDomainEvent(new PlayerDeposited(
                    PlayerId,
                    BannerTag.Value,
                    amount,
                    pixelEvent.Timestamp,
                    sourceIp,
                    userAgent,
                    isFirstDeposit));
            }

            IncrementVersion();
            return pixelEvent;
        }

        /// <summary>
        /// Gets events by type
        /// </summary>
        /// <param name="eventType">The event type to filter by</param>
        /// <returns>Collection of events of the specified type</returns>
        public IEnumerable<PixelEvent> GetEventsByType(EventType eventType)
        {
            return _pixelEvents.Where(e => e.EventType == eventType.Value);
        }

        /// <summary>
        /// Gets events within a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Collection of events within the date range</returns>
        public IEnumerable<PixelEvent> GetEventsInDateRange(DateTime startDate, DateTime endDate)
        {
            return _pixelEvents.Where(e => e.Timestamp >= startDate && e.Timestamp <= endDate);
        }

        /// <summary>
        /// Gets the count of events by type
        /// </summary>
        /// <param name="eventType">The event type to count</param>
        /// <returns>The count of events</returns>
        public int GetEventCount(EventType eventType)
        {
            return _pixelEvents.Count(e => e.EventType == eventType.Value);
        }

        /// <summary>
        /// Checks if the player has registered
        /// </summary>
        /// <returns>True if the player has registered, false otherwise</returns>
        public bool HasRegistered()
        {
            return _pixelEvents.Any(e => e.EventType == EventType.Registration.Value);
        }

        /// <summary>
        /// Checks if the player has deposited
        /// </summary>
        /// <returns>True if the player has deposited, false otherwise</returns>
        public bool HasDeposited()
        {
            return _pixelEvents.Any(e => e.EventType == EventType.Deposit.Value);
        }

        /// <summary>
        /// Gets the total deposit amount
        /// </summary>
        /// <returns>The total deposit amount</returns>
        public decimal GetTotalDepositAmount()
        {
            return _pixelEvents
                .Where(e => e.EventType == EventType.Deposit.Value)
                .Sum(e => e.Metadata.ContainsKey("amount") && decimal.TryParse(e.Metadata["amount"], out var amount) ? amount : 0m);
        }
    }
}

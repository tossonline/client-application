using System;
using System.Collections.Generic;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when a pixel event is received and processed
    /// </summary>
    public class PixelEventReceived : DomainEvent
    {
        /// <summary>
        /// Gets the type of event that was received
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// Gets the unique identifier of the player
        /// </summary>
        public string PlayerId { get; }

        /// <summary>
        /// Gets the banner tag associated with the event
        /// </summary>
        public string BannerTag { get; }

        /// <summary>
        /// Gets the additional metadata associated with the event
        /// </summary>
        public Dictionary<string, string> Metadata { get; }

        /// <summary>
        /// Gets the source IP address of the event
        /// </summary>
        public string? SourceIp { get; }

        /// <summary>
        /// Gets the user agent string from the event
        /// </summary>
        public string? UserAgent { get; }

        /// <summary>
        /// Gets the timestamp when the event occurred
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the PixelEventReceived class
        /// </summary>
        /// <param name="eventType">The type of event</param>
        /// <param name="playerId">The player identifier</param>
        /// <param name="bannerTag">The banner tag</param>
        /// <param name="metadata">The event metadata</param>
        /// <param name="sourceIp">The source IP address</param>
        /// <param name="userAgent">The user agent string</param>
        /// <param name="timestamp">The event timestamp</param>
        public PixelEventReceived(
            string eventType,
            string playerId,
            string bannerTag,
            Dictionary<string, string> metadata,
            string? sourceIp,
            string? userAgent,
            DateTime timestamp)
        {
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            PlayerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
            BannerTag = bannerTag ?? throw new ArgumentNullException(nameof(bannerTag));
            Metadata = metadata ?? new Dictionary<string, string>();
            SourceIp = sourceIp;
            UserAgent = userAgent;
            Timestamp = timestamp;
        }
    }
} 
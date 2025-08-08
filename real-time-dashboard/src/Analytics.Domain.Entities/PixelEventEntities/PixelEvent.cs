using System;
using System.Collections.Generic;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents a pixel event from tracking systems.
    /// This entity captures user interactions and activities tracked via tracking pixels.
    /// </summary>
    /// <remarks>
    /// Pixel events are the foundation of the analytics system, capturing all user interactions
    /// such as visits, registrations, and deposits. Each event includes metadata about the
    /// interaction and is used for real-time analytics and historical reporting.
    /// </remarks>
    public class PixelEvent
    {
        /// <summary>
        /// Valid event types that can be tracked
        /// </summary>
        private static readonly HashSet<string> ValidEventTypes = new()
        {
            "visit",
            "registration",
            "deposit"
        };

        /// <summary>
        /// Gets the unique identifier for the event
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the type of event (e.g., visit, registration, deposit)
        /// </summary>
        /// <remarks>
        /// Event types are restricted to a predefined set to ensure data consistency
        /// </remarks>
        public string EventType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the unique identifier of the player who triggered the event
        /// </summary>
        public string PlayerId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the banner tag associated with the event
        /// </summary>
        /// <remarks>
        /// Banner tags follow the format: campaign_id-placement-size
        /// Example: campaign1-sidebar-300x250
        /// </remarks>
        public string BannerTag { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the campaign identifier extracted from the banner tag
        /// </summary>
        public string CampaignId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets additional metadata associated with the event
        /// </summary>
        /// <remarks>
        /// Metadata can include browser information, device details, or event-specific data
        /// like deposit amounts
        /// </remarks>
        public Dictionary<string, string> Metadata { get; private set; } = new();

        /// <summary>
        /// Gets the IP address of the user who triggered the event
        /// </summary>
        public string? SourceIp { get; private set; }

        /// <summary>
        /// Gets the user agent string of the browser/device that triggered the event
        /// </summary>
        public string? UserAgent { get; private set; }

        /// <summary>
        /// Gets the timestamp when the event occurred
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private PixelEvent() { }

        /// <summary>
        /// Creates a visit event
        /// </summary>
        /// <param name="playerId">The unique identifier of the player</param>
        /// <param name="bannerTag">The banner tag that was clicked</param>
        /// <param name="sourceIp">Optional. The IP address of the visitor</param>
        /// <param name="userAgent">Optional. The user agent string of the visitor's browser</param>
        /// <returns>A new PixelEvent instance representing a visit</returns>
        /// <exception cref="ArgumentException">Thrown when playerId or bannerTag is null or empty</exception>
        public static PixelEvent CreateVisit(string playerId, string bannerTag, string? sourceIp = null, string? userAgent = null)
        {
            return Create("visit", playerId, bannerTag, sourceIp, userAgent);
        }

        /// <summary>
        /// Creates a registration event
        /// </summary>
        /// <param name="playerId">The unique identifier of the player</param>
        /// <param name="bannerTag">The banner tag that led to the registration</param>
        /// <param name="sourceIp">Optional. The IP address of the registrant</param>
        /// <param name="userAgent">Optional. The user agent string of the registrant's browser</param>
        /// <returns>A new PixelEvent instance representing a registration</returns>
        /// <exception cref="ArgumentException">Thrown when playerId or bannerTag is null or empty</exception>
        public static PixelEvent CreateRegistration(string playerId, string bannerTag, string? sourceIp = null, string? userAgent = null)
        {
            return Create("registration", playerId, bannerTag, sourceIp, userAgent);
        }

        /// <summary>
        /// Creates a deposit event
        /// </summary>
        /// <param name="playerId">The unique identifier of the player</param>
        /// <param name="bannerTag">The banner tag associated with the deposit</param>
        /// <param name="sourceIp">Optional. The IP address of the depositor</param>
        /// <param name="userAgent">Optional. The user agent string of the depositor's browser</param>
        /// <returns>A new PixelEvent instance representing a deposit</returns>
        /// <exception cref="ArgumentException">Thrown when playerId or bannerTag is null or empty</exception>
        public static PixelEvent CreateDeposit(string playerId, string bannerTag, string? sourceIp = null, string? userAgent = null)
        {
            return Create("deposit", playerId, bannerTag, sourceIp, userAgent);
        }

        /// <summary>
        /// Creates a new pixel event
        /// </summary>
        /// <param name="eventType">The type of event</param>
        /// <param name="playerId">The unique identifier of the player</param>
        /// <param name="bannerTag">The banner tag associated with the event</param>
        /// <param name="sourceIp">Optional. The IP address of the user</param>
        /// <param name="userAgent">Optional. The user agent string of the user's browser</param>
        /// <returns>A new PixelEvent instance</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// - eventType is not one of the valid types
        /// - playerId is null or empty
        /// - bannerTag is null or empty
        /// </exception>
        private static PixelEvent Create(string eventType, string playerId, string bannerTag, string? sourceIp, string? userAgent)
        {
            if (!ValidEventTypes.Contains(eventType))
                throw new ArgumentException($"Invalid event type. Must be one of: {string.Join(", ", ValidEventTypes)}", nameof(eventType));

            if (string.IsNullOrWhiteSpace(playerId))
                throw new ArgumentException("Player ID cannot be null or empty", nameof(playerId));

            if (string.IsNullOrWhiteSpace(bannerTag))
                throw new ArgumentException("Banner tag cannot be null or empty", nameof(bannerTag));

            return new PixelEvent
            {
                EventType = eventType,
                PlayerId = playerId,
                BannerTag = bannerTag,
                CampaignId = ExtractCampaignId(bannerTag),
                SourceIp = sourceIp,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Adds metadata to the event
        /// </summary>
        /// <param name="key">The metadata key</param>
        /// <param name="value">The metadata value</param>
        /// <exception cref="ArgumentException">Thrown when key is null or empty</exception>
        public void AddMetadata(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Metadata key cannot be null or empty", nameof(key));

            Metadata[key] = value;
        }

        /// <summary>
        /// Extracts the campaign ID from a banner tag
        /// </summary>
        /// <param name="bannerTag">The banner tag to parse</param>
        /// <returns>The extracted campaign ID</returns>
        /// <remarks>
        /// Banner tags follow the format: campaign_id-placement-size
        /// This method extracts the campaign_id part
        /// </remarks>
        private static string ExtractCampaignId(string bannerTag)
        {
            var parts = bannerTag.Split('-');
            return parts.Length > 0 ? parts[0] : bannerTag;
        }

        /// <summary>
        /// Checks if the event is valid
        /// </summary>
        /// <returns>True if the event is valid, false otherwise</returns>
        /// <remarks>
        /// An event is considered valid if it has:
        /// - A valid event type
        /// - A non-empty player ID
        /// - A non-empty banner tag
        /// </remarks>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EventType) &&
                   !string.IsNullOrWhiteSpace(PlayerId) &&
                   !string.IsNullOrWhiteSpace(BannerTag) &&
                   ValidEventTypes.Contains(EventType);
        }
    }
}
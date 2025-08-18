using System;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when a player completes registration
    /// </summary>
    public class PlayerRegistered : DomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the player
        /// </summary>
        public string PlayerId { get; }

        /// <summary>
        /// Gets the banner tag that led to the registration
        /// </summary>
        public string BannerTag { get; }

        /// <summary>
        /// Gets the timestamp when the registration occurred
        /// </summary>
        public DateTime RegistrationDate { get; }

        /// <summary>
        /// Gets the source IP address of the registration
        /// </summary>
        public string? SourceIp { get; }

        /// <summary>
        /// Gets the user agent string from the registration
        /// </summary>
        public string? UserAgent { get; }

        /// <summary>
        /// Initializes a new instance of the PlayerRegistered class
        /// </summary>
        /// <param name="playerId">The player identifier</param>
        /// <param name="bannerTag">The banner tag that led to registration</param>
        /// <param name="registrationDate">The registration timestamp</param>
        /// <param name="sourceIp">The source IP address</param>
        /// <param name="userAgent">The user agent string</param>
        public PlayerRegistered(
            string playerId,
            string bannerTag,
            DateTime registrationDate,
            string? sourceIp,
            string? userAgent)
        {
            PlayerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
            BannerTag = bannerTag ?? throw new ArgumentNullException(nameof(bannerTag));
            RegistrationDate = registrationDate;
            SourceIp = sourceIp;
            UserAgent = userAgent;
        }
    }
}

using System;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when a player makes a deposit
    /// </summary>
    public class PlayerDeposited : DomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the player
        /// </summary>
        public string PlayerId { get; }

        /// <summary>
        /// Gets the banner tag associated with the deposit
        /// </summary>
        public string BannerTag { get; }

        /// <summary>
        /// Gets the deposit amount
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Gets the timestamp when the deposit occurred
        /// </summary>
        public DateTime DepositDate { get; }

        /// <summary>
        /// Gets the source IP address of the deposit
        /// </summary>
        public string? SourceIp { get; }

        /// <summary>
        /// Gets the user agent string from the deposit
        /// </summary>
        public string? UserAgent { get; }

        /// <summary>
        /// Gets whether this is the player's first deposit
        /// </summary>
        public bool IsFirstDeposit { get; }

        /// <summary>
        /// Initializes a new instance of the PlayerDeposited class
        /// </summary>
        /// <param name="playerId">The player identifier</param>
        /// <param name="bannerTag">The banner tag associated with the deposit</param>
        /// <param name="amount">The deposit amount</param>
        /// <param name="depositDate">The deposit timestamp</param>
        /// <param name="sourceIp">The source IP address</param>
        /// <param name="userAgent">The user agent string</param>
        /// <param name="isFirstDeposit">Whether this is the player's first deposit</param>
        public PlayerDeposited(
            string playerId,
            string bannerTag,
            decimal amount,
            DateTime depositDate,
            string? sourceIp,
            string? userAgent,
            bool isFirstDeposit)
        {
            PlayerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
            BannerTag = bannerTag ?? throw new ArgumentNullException(nameof(bannerTag));
            Amount = amount;
            DepositDate = depositDate;
            SourceIp = sourceIp;
            UserAgent = userAgent;
            IsFirstDeposit = isFirstDeposit;
        }
    }
}

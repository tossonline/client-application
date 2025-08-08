using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents a player in the analytics system.
    /// This entity tracks player lifecycle events, status, and value metrics.
    /// </summary>
    /// <remarks>
    /// The Player entity is a core domain concept that maintains the state and history
    /// of player interactions. It supports player segmentation, value tracking, and
    /// lifecycle management through various status transitions.
    /// </remarks>
    public class Player
    {
        /// <summary>
        /// Gets the unique identifier for the player record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the unique identifier assigned to the player
        /// </summary>
        public string PlayerId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the timestamp when the player was first seen
        /// </summary>
        public DateTime FirstSeen { get; private set; }

        /// <summary>
        /// Gets the timestamp of the player's last event
        /// </summary>
        public DateTime? LastEventAt { get; private set; }

        /// <summary>
        /// Gets the timestamp when the player registered
        /// </summary>
        public DateTime? RegistrationDate { get; private set; }

        /// <summary>
        /// Gets the timestamp of the player's first deposit
        /// </summary>
        public DateTime? FirstDepositDate { get; private set; }

        /// <summary>
        /// Gets the total number of deposits made by the player
        /// </summary>
        public int TotalDeposits { get; private set; }

        /// <summary>
        /// Gets the total amount deposited by the player
        /// </summary>
        public decimal TotalDepositAmount { get; private set; }

        /// <summary>
        /// Gets the current status of the player
        /// </summary>
        public PlayerStatus Status { get; private set; }

        /// <summary>
        /// Gets the current segment of the player
        /// </summary>
        public PlayerSegment Segment { get; private set; }

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private Player() { }

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="playerId">The unique identifier for the player</param>
        /// <returns>A new Player instance</returns>
        /// <exception cref="ArgumentException">Thrown when playerId is null or empty</exception>
        public static Player Create(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                throw new ArgumentException("Player ID cannot be null or empty", nameof(playerId));

            return new Player
            {
                PlayerId = playerId,
                FirstSeen = DateTime.UtcNow,
                Status = PlayerStatus.Visitor,
                Segment = PlayerSegment.New,
                TotalDeposits = 0,
                TotalDepositAmount = 0
            };
        }

        /// <summary>
        /// Registers the player
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the player is already registered</exception>
        /// <remarks>
        /// Registration is a one-time event that transitions the player from Visitor to Registered status.
        /// This method also triggers segment re-evaluation.
        /// </remarks>
        public void Register()
        {
            if (RegistrationDate.HasValue)
                throw new InvalidOperationException("Player is already registered");

            RegistrationDate = DateTime.UtcNow;
            Status = PlayerStatus.Registered;
            UpdateSegment();
        }

        /// <summary>
        /// Records a deposit for the player
        /// </summary>
        /// <param name="amount">The deposit amount</param>
        /// <exception cref="InvalidOperationException">Thrown when the player is not registered</exception>
        /// <exception cref="ArgumentException">Thrown when amount is not positive</exception>
        /// <remarks>
        /// Deposits can only be made by registered players. The first deposit updates FirstDepositDate
        /// and subsequent deposits increment counters and update the total amount.
        /// </remarks>
        public void Deposit(decimal amount)
        {
            if (!RegistrationDate.HasValue)
                throw new InvalidOperationException("Player must be registered before depositing");

            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive", nameof(amount));

            if (!FirstDepositDate.HasValue)
                FirstDepositDate = DateTime.UtcNow;

            TotalDeposits++;
            TotalDepositAmount += amount;
            Status = PlayerStatus.Deposited;
            UpdateSegment();
        }

        /// <summary>
        /// Updates the last event timestamp
        /// </summary>
        /// <param name="eventTime">The timestamp of the event</param>
        /// <exception cref="ArgumentException">Thrown when eventTime is before FirstSeen</exception>
        /// <remarks>
        /// This method is called for every player event to maintain accurate activity tracking.
        /// It also triggers segment re-evaluation based on activity patterns.
        /// </remarks>
        public void UpdateLastEvent(DateTime eventTime)
        {
            if (eventTime < FirstSeen)
                throw new ArgumentException("Event time cannot be before first seen", nameof(eventTime));

            LastEventAt = eventTime;
            UpdateSegment();
        }

        /// <summary>
        /// Calculates player lifetime in days
        /// </summary>
        /// <returns>The number of days since the player was first seen</returns>
        public int GetLifetimeDays()
        {
            var lastActivity = LastEventAt ?? FirstSeen;
            return (int)(lastActivity - FirstSeen).TotalDays;
        }

        /// <summary>
        /// Calculates time to registration in minutes
        /// </summary>
        /// <returns>The number of minutes between first seen and registration, or null if not registered</returns>
        public double? GetTimeToRegistration()
        {
            if (!RegistrationDate.HasValue)
                return null;

            return (RegistrationDate.Value - FirstSeen).TotalMinutes;
        }

        /// <summary>
        /// Calculates time to first deposit in minutes
        /// </summary>
        /// <returns>The number of minutes between registration and first deposit, or null if not deposited</returns>
        public double? GetTimeToFirstDeposit()
        {
            if (!FirstDepositDate.HasValue || !RegistrationDate.HasValue)
                return null;

            return (FirstDepositDate.Value - RegistrationDate.Value).TotalMinutes;
        }

        /// <summary>
        /// Calculates average deposit amount
        /// </summary>
        /// <returns>The average amount per deposit, or 0 if no deposits</returns>
        public decimal GetAverageDepositAmount()
        {
            if (TotalDeposits == 0)
                return 0;

            return TotalDepositAmount / TotalDeposits;
        }

        /// <summary>
        /// Updates the player's segment based on current status and activity
        /// </summary>
        /// <remarks>
        /// Segment rules:
        /// - VIP: Deposited player with 5+ deposits
        /// - Regular: Deposited player with 2+ deposits
        /// - Inactive: No activity for 30+ days
        /// - NonDepositor: Registered for 7+ days with no deposits
        /// - New: All other players
        /// </remarks>
        private void UpdateSegment()
        {
            var lastActivity = LastEventAt ?? FirstSeen;
            var daysSinceLastActivity = (DateTime.UtcNow - lastActivity).TotalDays;

            Segment = (Status, daysSinceLastActivity, TotalDeposits) switch
            {
                (PlayerStatus.Deposited, _, >= 5) => PlayerSegment.VIP,
                (PlayerStatus.Deposited, _, >= 2) => PlayerSegment.Regular,
                (_, > 30, _) => PlayerSegment.Inactive,
                (PlayerStatus.Registered, > 7, 0) => PlayerSegment.NonDepositor,
                (_, _, _) => PlayerSegment.New
            };
        }
    }

    /// <summary>
    /// Represents the current status of a player
    /// </summary>
    public enum PlayerStatus
    {
        /// <summary>
        /// Player has visited but not registered
        /// </summary>
        Visitor,

        /// <summary>
        /// Player has registered but not made a deposit
        /// </summary>
        Registered,

        /// <summary>
        /// Player has made at least one deposit
        /// </summary>
        Deposited
    }

    /// <summary>
    /// Represents the player's segment based on behavior and value
    /// </summary>
    public enum PlayerSegment
    {
        /// <summary>
        /// New or recently active player
        /// </summary>
        New,

        /// <summary>
        /// Player with consistent deposit activity
        /// </summary>
        Regular,

        /// <summary>
        /// High-value player with frequent deposits
        /// </summary>
        VIP,

        /// <summary>
        /// Registered player who hasn't made a deposit
        /// </summary>
        NonDepositor,

        /// <summary>
        /// Player with no recent activity
        /// </summary>
        Inactive
    }
}
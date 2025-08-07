using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents a player in the analytics system
    /// </summary>
    public class Player
    {
        public int Id { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public DateTime FirstSeen { get; set; }
        public DateTime? LastEventAt { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? FirstDepositDate { get; set; }

        /// <summary>
        /// Create a new player
        /// </summary>
        public static Player Create(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                throw new ArgumentException("Player ID cannot be null or empty", nameof(playerId));

            return new Player
            {
                PlayerId = playerId,
                FirstSeen = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Register the player
        /// </summary>
        public void Register()
        {
            RegistrationDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Record a deposit for the player
        /// </summary>
        public void Deposit()
        {
            if (!RegistrationDate.HasValue)
                throw new InvalidOperationException("Player must be registered before depositing");

            if (!FirstDepositDate.HasValue)
                FirstDepositDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Update the last event timestamp
        /// </summary>
        public void UpdateLastEvent(DateTime eventTime)
        {
            LastEventAt = eventTime;
        }
    }
}

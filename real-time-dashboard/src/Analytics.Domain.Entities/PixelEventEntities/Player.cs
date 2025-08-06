using System;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents a player and their lifecycle events.
    /// </summary>
    public class Player : IPlayer
    {
        public string PlayerId { get; private set; }
        public DateTime FirstSeen { get; private set; }
        public DateTime? LastEventAt { get; private set; }
        public DateTime? RegistrationAt { get; private set; }
        public DateTime? DepositAt { get; private set; }
        public int TotalDeposits { get; private set; }

        private Player(string playerId, DateTime firstSeen)
        {
            PlayerId = playerId;
            FirstSeen = firstSeen;
        }

        public static Player Create(string playerId)
        {
            return new Player(playerId, DateTime.UtcNow);
        }

        public void Register()
        {
            if (RegistrationAt.HasValue)
                throw new InvalidOperationException("Player already registered.");
            RegistrationAt = DateTime.UtcNow;
            UpdateLastEvent();
        }

        public void Deposit()
        {
            if (!RegistrationAt.HasValue)
                throw new InvalidOperationException("Player must be registered before depositing.");
            if (!DepositAt.HasValue)
                DepositAt = DateTime.UtcNow;
            TotalDeposits++;
            UpdateLastEvent();
        }

        public void UpdateLastEvent()
        {
            LastEventAt = DateTime.UtcNow;
        }
    }
}

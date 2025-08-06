using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Entities.PlayerAggregate
{
    /// <summary>
    /// Aggregate root for player lifecycle management
    /// </summary>
    public class PlayerAggregate
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPixelEventRepository _pixelEventRepository;

        public PlayerAggregate(
            IPlayerRepository playerRepository,
            IPixelEventRepository pixelEventRepository)
        {
            _playerRepository = playerRepository;
            _pixelEventRepository = pixelEventRepository;
        }

        public async Task<Player> GetOrCreatePlayerAsync(string playerId)
        {
            var player = await _playerRepository.GetByPlayerIdAsync(playerId);
            
            if (player == null)
            {
                var newPlayer = Player.Create(playerId);
                await _playerRepository.AddAsync(newPlayer);
                return newPlayer;
            }

            return (Player)player; // Cast to concrete type
        }

        public async Task UpdatePlayerFromEventAsync(string playerId, string eventType)
        {
            var player = await GetOrCreatePlayerAsync(playerId);
            
            player.UpdateLastEvent();

            switch (eventType.ToLower())
            {
                case "registration":
                    player.Register();
                    break;
                case "deposit":
                    if (player.RegistrationAt.HasValue)
                    {
                        player.Deposit();
                    }
                    break;
            }

            await _playerRepository.UpdateAsync(player);
        }

        public async Task<PlayerStats> GetPlayerStatsAsync(string playerId)
        {
            var player = await _playerRepository.GetByPlayerIdAsync(playerId);
            if (player == null)
            {
                return null;
            }

            var events = await _pixelEventRepository.GetByPlayerIdAsync(playerId);
            
            return new PlayerStats
            {
                PlayerId = player.PlayerId,
                FirstSeen = player.FirstSeen,
                LastEventAt = player.LastEventAt,
                RegistrationAt = player.RegistrationAt,
                DepositAt = player.DepositAt,
                TotalDeposits = player.TotalDeposits,
                TotalEvents = events.Count(),
                EventBreakdown = events.GroupBy(e => e.EventType)
                    .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        public async Task<List<Player>> GetPlayersByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            // This would need to be implemented in the repository
            // For now, we'll return an empty list
            return new List<Player>();
        }
    }

    public class PlayerStats
    {
        public string PlayerId { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime? LastEventAt { get; set; }
        public DateTime? RegistrationAt { get; set; }
        public DateTime? DepositAt { get; set; }
        public int TotalDeposits { get; set; }
        public int TotalEvents { get; set; }
        public Dictionary<string, int> EventBreakdown { get; set; } = new();
    }
}
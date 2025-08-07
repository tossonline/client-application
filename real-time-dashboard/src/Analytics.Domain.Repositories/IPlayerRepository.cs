using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    /// <summary>
    /// Repository interface for Player entity operations
    /// </summary>
    public interface IPlayerRepository
    {
        /// <summary>
        /// Get player by player ID
        /// </summary>
        Task<Player> GetByPlayerIdAsync(string playerId);

        /// <summary>
        /// Add a new player
        /// </summary>
        Task AddAsync(Player player);

        /// <summary>
        /// Update an existing player
        /// </summary>
        Task UpdateAsync(Player player);

        /// <summary>
        /// Get all players
        /// </summary>
        Task<IEnumerable<Player>> GetAllAsync();

        /// <summary>
        /// Get players by registration date range
        /// </summary>
        Task<IEnumerable<Player>> GetByRegistrationDateRangeAsync(System.DateTime startDate, System.DateTime endDate);

        /// <summary>
        /// Get players by first deposit date range
        /// </summary>
        Task<IEnumerable<Player>> GetByFirstDepositDateRangeAsync(System.DateTime startDate, System.DateTime endDate);
    }
}


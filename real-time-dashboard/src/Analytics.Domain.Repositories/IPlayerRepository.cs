using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetByPlayerIdAsync(string playerId);
        Task AddAsync(Player player);
        Task UpdateAsync(Player player);
        Task DeleteAsync(string playerId);
    }
} 
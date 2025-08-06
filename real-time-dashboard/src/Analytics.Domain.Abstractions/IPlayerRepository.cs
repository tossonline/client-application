using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    public interface IPlayerRepository
    {
        Task<IPlayer> GetByPlayerIdAsync(string playerId);
        Task AddAsync(IPlayer player);
        Task UpdateAsync(IPlayer player);
        Task DeleteAsync(string playerId);
    }

    public interface IPlayer
    {
        string PlayerId { get; }
        System.DateTime FirstSeen { get; }
        System.DateTime? LastEventAt { get; }
        System.DateTime? RegistrationAt { get; }
        System.DateTime? DepositAt { get; }
        int TotalDeposits { get; }
        void Register();
        void Deposit();
        void UpdateLastEvent();
    }
}
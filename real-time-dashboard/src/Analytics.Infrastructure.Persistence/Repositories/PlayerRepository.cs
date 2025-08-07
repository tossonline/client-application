using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Analytics.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AnalyticsContext _context;

        public PlayerRepository(AnalyticsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Player> GetByPlayerIdAsync(string playerId)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Players
                .Where(p => p.RegistrationDate.HasValue && 
                           p.RegistrationDate.Value.Date >= startDate.Date && 
                           p.RegistrationDate.Value.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetByFirstDepositDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Players
                .Where(p => p.FirstDepositDate.HasValue && 
                           p.FirstDepositDate.Value.Date >= startDate.Date && 
                           p.FirstDepositDate.Value.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for PixelEvent entity
    /// </summary>
    public class PixelEventRepository : IPixelEventRepository
    {
        private readonly AnalyticsContext _context;

        public PixelEventRepository(AnalyticsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PixelEvent> GetByIdAsync(int id)
        {
            return await _context.PixelEvents.FindAsync(id);
        }

        public async Task<IEnumerable<PixelEvent>> GetAllAsync()
        {
            return await _context.PixelEvents.ToListAsync();
        }

        public async Task<IEnumerable<PixelEvent>> GetByDateAsync(DateTime date)
        {
            return await _context.PixelEvents
                .Where(e => e.Timestamp.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<PixelEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.PixelEvents
                .Where(e => e.Timestamp.Date >= startDate.Date && e.Timestamp.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<PixelEvent>> GetByPlayerIdAsync(string playerId)
        {
            return await _context.PixelEvents
                .Where(e => e.PlayerId == playerId)
                .OrderByDescending(e => e.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<PixelEvent>> GetByEventTypeAsync(string eventType)
        {
            return await _context.PixelEvents
                .Where(e => e.EventType == eventType)
                .ToListAsync();
        }

        public async Task<IEnumerable<PixelEvent>> GetByBannerTagAsync(string bannerTag)
        {
            return await _context.PixelEvents
                .Where(e => e.BannerTag == bannerTag)
                .ToListAsync();
        }

        public async Task AddAsync(PixelEvent pixelEvent)
        {
            await _context.PixelEvents.AddAsync(pixelEvent);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PixelEvent pixelEvent)
        {
            _context.PixelEvents.Update(pixelEvent);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PixelEvent pixelEvent)
        {
            _context.PixelEvents.Remove(pixelEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PixelEvents.AnyAsync(e => e.Id == id);
        }
    }
}


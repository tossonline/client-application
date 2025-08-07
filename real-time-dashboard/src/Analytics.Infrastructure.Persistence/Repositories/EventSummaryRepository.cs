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
    public class EventSummaryRepository : IEventSummaryRepository
    {
        private readonly AnalyticsContext _context;

        public EventSummaryRepository(AnalyticsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(EventSummary eventSummary)
        {
            await _context.EventSummaries.AddAsync(eventSummary);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventSummary>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.EventSummaries
                .Where(e => e.EventDate >= startDate.Date && e.EventDate <= endDate.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventSummary>> GetByDateAndEventTypeAsync(DateTime date, string eventType)
        {
            return await _context.EventSummaries
                .Where(e => e.EventDate == date.Date && e.EventType == eventType)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventSummary>> GetByDateAndBannerTagAsync(DateTime date, string bannerTag)
        {
            return await _context.EventSummaries
                .Where(e => e.EventDate == date.Date && e.BannerTag == bannerTag)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventSummary>> GetByDateEventTypeAndBannerTagAsync(DateTime date, string eventType, string bannerTag)
        {
            return await _context.EventSummaries
                .Where(e => e.EventDate == date.Date && e.EventType == eventType && e.BannerTag == bannerTag)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventSummary>> GetByDateAsync(DateTime date)
        {
            return await _context.EventSummaries
                .Where(e => e.EventDate == date.Date)
                .ToListAsync();
        }

        public async Task UpdateAsync(EventSummary eventSummary)
        {
            _context.EventSummaries.Update(eventSummary);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByDateAsync(DateTime date)
        {
            var summariesToDelete = await _context.EventSummaries
                .Where(e => e.EventDate == date.Date)
                .ToListAsync();

            _context.EventSummaries.RemoveRange(summariesToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
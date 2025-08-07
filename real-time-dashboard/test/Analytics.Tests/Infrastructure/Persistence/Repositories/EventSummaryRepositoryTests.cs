using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Persistence.Contexts;
using Analytics.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Analytics.Tests.Infrastructure.Persistence.Repositories
{
    [TestFixture]
    public class EventSummaryRepositoryTests
    {
        private DbContextOptions<AnalyticsContext> _options;
        private AnalyticsContext _context;
        private EventSummaryRepository _repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _options = new DbContextOptionsBuilder<AnalyticsContext>()
                .UseInMemoryDatabase("EventSummaryRepositoryTests")
                .Options;
        }

        [SetUp]
        public void Setup()
        {
            _context = new AnalyticsContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new EventSummaryRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetByDateRangeAsync_ReturnsSummariesInRange()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date;
            var endDate = startDate.AddDays(2);
            var summaries = new List<EventSummary>
            {
                new EventSummary { EventDate = startDate, EventType = "visit", Count = 10 },
                new EventSummary { EventDate = startDate.AddDays(1), EventType = "visit", Count = 20 },
                new EventSummary { EventDate = startDate.AddDays(3), EventType = "visit", Count = 30 }
            };
            await _context.EventSummaries.AddRangeAsync(summaries);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(s => s.EventDate >= startDate && s.EventDate <= endDate), Is.True);
        }

        [Test]
        public async Task GetByDateAndEventTypeAsync_ReturnsSummariesForDateAndType()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var summaries = new List<EventSummary>
            {
                new EventSummary { EventDate = date, EventType = "visit", Count = 10 },
                new EventSummary { EventDate = date, EventType = "registration", Count = 5 },
                new EventSummary { EventDate = date.AddDays(1), EventType = "visit", Count = 15 }
            };
            await _context.EventSummaries.AddRangeAsync(summaries);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateAndEventTypeAsync(date, "visit");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Count, Is.EqualTo(10));
        }

        [Test]
        public async Task GetByDateAndBannerTagAsync_ReturnsSummariesForDateAndBanner()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var bannerTag = "banner-123";
            var summaries = new List<EventSummary>
            {
                new EventSummary { EventDate = date, EventType = "visit", BannerTag = bannerTag, Count = 10 },
                new EventSummary { EventDate = date, EventType = "visit", BannerTag = "other-banner", Count = 5 },
                new EventSummary { EventDate = date.AddDays(1), EventType = "visit", BannerTag = bannerTag, Count = 15 }
            };
            await _context.EventSummaries.AddRangeAsync(summaries);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateAndBannerTagAsync(date, bannerTag);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Count, Is.EqualTo(10));
        }

        [Test]
        public async Task DeleteByDateAsync_RemovesSummariesForDate()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var summaries = new List<EventSummary>
            {
                new EventSummary { EventDate = date, EventType = "visit", Count = 10 },
                new EventSummary { EventDate = date, EventType = "registration", Count = 5 },
                new EventSummary { EventDate = date.AddDays(1), EventType = "visit", Count = 15 }
            };
            await _context.EventSummaries.AddRangeAsync(summaries);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteByDateAsync(date);

            // Assert
            var remainingSummaries = await _context.EventSummaries.ToListAsync();
            Assert.That(remainingSummaries.Count, Is.EqualTo(1));
            Assert.That(remainingSummaries.All(s => s.EventDate != date), Is.True);
        }
    }
}

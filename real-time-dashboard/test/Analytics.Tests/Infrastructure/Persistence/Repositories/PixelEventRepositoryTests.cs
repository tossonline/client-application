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
    public class PixelEventRepositoryTests
    {
        private DbContextOptions<AnalyticsContext> _options;
        private AnalyticsContext _context;
        private PixelEventRepository _repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _options = new DbContextOptionsBuilder<AnalyticsContext>()
                .UseInMemoryDatabase("PixelEventRepositoryTests")
                .Options;
        }

        [SetUp]
        public void Setup()
        {
            _context = new AnalyticsContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _repository = new PixelEventRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetByIdAsync_ExistingEvent_ReturnsEvent()
        {
            // Arrange
            var pixelEvent = new PixelEvent
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };
            await _context.PixelEvents.AddAsync(pixelEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(pixelEvent.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(pixelEvent.Id));
            Assert.That(result.EventType, Is.EqualTo(pixelEvent.EventType));
            Assert.That(result.PlayerId, Is.EqualTo(pixelEvent.PlayerId));
            Assert.That(result.BannerTag, Is.EqualTo(pixelEvent.BannerTag));
        }

        [Test]
        public async Task GetByDateAsync_ReturnsEventsForDate()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var events = new List<PixelEvent>
            {
                new PixelEvent { EventType = "visit", Timestamp = date.AddHours(1) },
                new PixelEvent { EventType = "visit", Timestamp = date.AddHours(2) },
                new PixelEvent { EventType = "visit", Timestamp = date.AddDays(1) }
            };
            await _context.PixelEvents.AddRangeAsync(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateAsync(date);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(e => e.Timestamp.Date == date), Is.True);
        }

        [Test]
        public async Task GetByPlayerIdAsync_ReturnsPlayerEvents()
        {
            // Arrange
            var playerId = "player-123";
            var events = new List<PixelEvent>
            {
                new PixelEvent { EventType = "visit", PlayerId = playerId, Timestamp = DateTime.UtcNow },
                new PixelEvent { EventType = "registration", PlayerId = playerId, Timestamp = DateTime.UtcNow },
                new PixelEvent { EventType = "visit", PlayerId = "other-player", Timestamp = DateTime.UtcNow }
            };
            await _context.PixelEvents.AddRangeAsync(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPlayerIdAsync(playerId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(e => e.PlayerId == playerId), Is.True);
        }

        [Test]
        public async Task AddAsync_ValidEvent_AddsToDatabase()
        {
            // Arrange
            var pixelEvent = new PixelEvent
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };

            // Act
            await _repository.AddAsync(pixelEvent);

            // Assert
            var savedEvent = await _context.PixelEvents.FindAsync(pixelEvent.Id);
            Assert.That(savedEvent, Is.Not.Null);
            Assert.That(savedEvent.EventType, Is.EqualTo(pixelEvent.EventType));
            Assert.That(savedEvent.PlayerId, Is.EqualTo(pixelEvent.PlayerId));
            Assert.That(savedEvent.BannerTag, Is.EqualTo(pixelEvent.BannerTag));
        }

        [Test]
        public async Task UpdateAsync_ExistingEvent_UpdatesDatabase()
        {
            // Arrange
            var pixelEvent = new PixelEvent
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };
            await _context.PixelEvents.AddAsync(pixelEvent);
            await _context.SaveChangesAsync();

            // Act
            pixelEvent.EventType = "registration";
            await _repository.UpdateAsync(pixelEvent);

            // Assert
            var updatedEvent = await _context.PixelEvents.FindAsync(pixelEvent.Id);
            Assert.That(updatedEvent.EventType, Is.EqualTo("registration"));
        }

        [Test]
        public async Task DeleteAsync_ExistingEvent_RemovesFromDatabase()
        {
            // Arrange
            var pixelEvent = new PixelEvent
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };
            await _context.PixelEvents.AddAsync(pixelEvent);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(pixelEvent);

            // Assert
            var deletedEvent = await _context.PixelEvents.FindAsync(pixelEvent.Id);
            Assert.That(deletedEvent, Is.Null);
        }
    }
}

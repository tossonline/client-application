using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Application.Handlers;
using Analytics.Domain.Commands;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Analytics.Tests.Application.Handlers
{
    [TestFixture]
    public class AggregateEventsHandlerTests
    {
        private Mock<IPixelEventRepository> _mockPixelEventRepository;
        private Mock<IEventSummaryRepository> _mockEventSummaryRepository;
        private Mock<ILogger<AggregateEventsHandler>> _mockLogger;
        private AggregateEventsHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockPixelEventRepository = new Mock<IPixelEventRepository>();
            _mockEventSummaryRepository = new Mock<IEventSummaryRepository>();
            _mockLogger = new Mock<ILogger<AggregateEventsHandler>>();
            _handler = new AggregateEventsHandler(_mockPixelEventRepository.Object, _mockEventSummaryRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_WithValidCommand_ShouldSucceed()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var command = new AggregateEventsCommand { EventDate = eventDate };

            var events = new List<PixelEvent>
            {
                new PixelEvent { EventType = "visit", BannerTag = "brandA", Timestamp = eventDate },
                new PixelEvent { EventType = "visit", BannerTag = "brandA", Timestamp = eventDate },
                new PixelEvent { EventType = "registration", BannerTag = "brandA", Timestamp = eventDate },
                new PixelEvent { EventType = "visit", BannerTag = "brandB", Timestamp = eventDate }
            };

            _mockPixelEventRepository.Setup(x => x.GetByDateAsync(eventDate))
                .ReturnsAsync(events);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockEventSummaryRepository.Verify(x => x.AddAsync(It.IsAny<EventSummary>()), Times.Exactly(3));
        }

        [Test]
        public async Task Handle_WithNoEvents_ShouldNotAddSummaries()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var command = new AggregateEventsCommand { EventDate = eventDate };

            _mockPixelEventRepository.Setup(x => x.GetByDateAsync(eventDate))
                .ReturnsAsync(new List<PixelEvent>());

            // Act
            await _handler.Handle(command);

            // Assert
            _mockEventSummaryRepository.Verify(x => x.AddAsync(It.IsAny<EventSummary>()), Times.Never);
        }

        [Test]
        public async Task Handle_WithMultipleEventTypes_ShouldCreateCorrectSummaries()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var command = new AggregateEventsCommand { EventDate = eventDate };

            var events = new List<PixelEvent>
            {
                new PixelEvent { EventType = "visit", BannerTag = "brandA", Timestamp = eventDate },
                new PixelEvent { EventType = "visit", BannerTag = "brandA", Timestamp = eventDate },
                new PixelEvent { EventType = "registration", BannerTag = "brandA", Timestamp = eventDate },
                new PixelEvent { EventType = "deposit", BannerTag = "brandA", Timestamp = eventDate }
            };

            _mockPixelEventRepository.Setup(x => x.GetByDateAsync(eventDate))
                .ReturnsAsync(events);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockEventSummaryRepository.Verify(x => x.AddAsync(It.Is<EventSummary>(s => 
                s.EventType == "visit" && s.BannerTag == "brandA" && s.Count == 2)), Times.Once);
            _mockEventSummaryRepository.Verify(x => x.AddAsync(It.Is<EventSummary>(s => 
                s.EventType == "registration" && s.BannerTag == "brandA" && s.Count == 1)), Times.Once);
            _mockEventSummaryRepository.Verify(x => x.AddAsync(It.Is<EventSummary>(s => 
                s.EventType == "deposit" && s.BannerTag == "brandA" && s.Count == 1)), Times.Once);
        }

        [Test]
        public async Task Handle_WithNullBannerTags_ShouldCreateSummaries()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var command = new AggregateEventsCommand { EventDate = eventDate };

            var events = new List<PixelEvent>
            {
                new PixelEvent { EventType = "visit", BannerTag = null, Timestamp = eventDate },
                new PixelEvent { EventType = "visit", BannerTag = null, Timestamp = eventDate }
            };

            _mockPixelEventRepository.Setup(x => x.GetByDateAsync(eventDate))
                .ReturnsAsync(events);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockEventSummaryRepository.Verify(x => x.AddAsync(It.Is<EventSummary>(s => 
                s.EventType == "visit" && s.BannerTag == null && s.Count == 2)), Times.Once);
        }

        [Test]
        public void Handle_WithNullCommand_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null));
            Assert.That(ex.ParamName, Is.EqualTo("command"));
        }

        [Test]
        public async Task Handle_WithRepositoryException_ShouldPropagateException()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var command = new AggregateEventsCommand { EventDate = eventDate };

            _mockPixelEventRepository.Setup(x => x.GetByDateAsync(eventDate))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command));
            Assert.That(ex.Message, Is.EqualTo("Database error"));
        }
    }
}


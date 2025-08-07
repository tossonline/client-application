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
        private Mock<IPixelEventRepository> _pixelEventRepositoryMock;
        private Mock<IEventSummaryRepository> _eventSummaryRepositoryMock;
        private Mock<ILogger<AggregateEventsHandler>> _loggerMock;
        private AggregateEventsHandler _handler;

        [SetUp]
        public void Setup()
        {
            _pixelEventRepositoryMock = new Mock<IPixelEventRepository>();
            _eventSummaryRepositoryMock = new Mock<IEventSummaryRepository>();
            _loggerMock = new Mock<ILogger<AggregateEventsHandler>>();
            _handler = new AggregateEventsHandler(
                _pixelEventRepositoryMock.Object,
                _eventSummaryRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ValidCommand_CreatesEventSummaries()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var command = new AggregateEventsCommand
            {
                EventDate = date
            };

            var events = new List<PixelEvent>
            {
                new PixelEvent { EventType = "visit", BannerTag = "banner1", Timestamp = date },
                new PixelEvent { EventType = "visit", BannerTag = "banner1", Timestamp = date },
                new PixelEvent { EventType = "registration", BannerTag = "banner1", Timestamp = date },
                new PixelEvent { EventType = "visit", BannerTag = "banner2", Timestamp = date }
            };

            _pixelEventRepositoryMock.Setup(x => x.GetByDateAsync(date))
                .ReturnsAsync(events);

            // Act
            await _handler.Handle(command);

            // Assert
            _eventSummaryRepositoryMock.Verify(
                x => x.AddAsync(It.Is<EventSummary>(s =>
                    s.EventDate == date &&
                    s.EventType == "visit" &&
                    s.BannerTag == "banner1" &&
                    s.Count == 2)),
                Times.Once);

            _eventSummaryRepositoryMock.Verify(
                x => x.AddAsync(It.Is<EventSummary>(s =>
                    s.EventDate == date &&
                    s.EventType == "registration" &&
                    s.BannerTag == "banner1" &&
                    s.Count == 1)),
                Times.Once);

            _eventSummaryRepositoryMock.Verify(
                x => x.AddAsync(It.Is<EventSummary>(s =>
                    s.EventDate == date &&
                    s.EventType == "visit" &&
                    s.BannerTag == "banner2" &&
                    s.Count == 1)),
                Times.Once);
        }

        [Test]
        public async Task Handle_NoEvents_DoesNotCreateSummaries()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var command = new AggregateEventsCommand
            {
                EventDate = date
            };

            _pixelEventRepositoryMock.Setup(x => x.GetByDateAsync(date))
                .ReturnsAsync(new List<PixelEvent>());

            // Act
            await _handler.Handle(command);

            // Assert
            _eventSummaryRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<EventSummary>()),
                Times.Never);
        }

        [Test]
        public async Task Handle_DeletesExistingSummaries()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var command = new AggregateEventsCommand
            {
                EventDate = date
            };

            _pixelEventRepositoryMock.Setup(x => x.GetByDateAsync(date))
                .ReturnsAsync(new List<PixelEvent>());

            // Act
            await _handler.Handle(command);

            // Assert
            _eventSummaryRepositoryMock.Verify(
                x => x.DeleteByDateAsync(date),
                Times.Once);
        }
    }
}
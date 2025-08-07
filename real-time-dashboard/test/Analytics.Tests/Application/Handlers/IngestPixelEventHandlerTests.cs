using System;
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
    public class IngestPixelEventHandlerTests
    {
        private Mock<IPixelEventRepository> _pixelEventRepositoryMock;
        private Mock<IPlayerRepository> _playerRepositoryMock;
        private Mock<ILogger<IngestPixelEventHandler>> _loggerMock;
        private IngestPixelEventHandler _handler;

        [SetUp]
        public void Setup()
        {
            _pixelEventRepositoryMock = new Mock<IPixelEventRepository>();
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _loggerMock = new Mock<ILogger<IngestPixelEventHandler>>();
            _handler = new IngestPixelEventHandler(
                _pixelEventRepositoryMock.Object,
                _playerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ValidCommand_CreatesPixelEventAndPlayer()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                SourceIp = "127.0.0.1",
                UserAgent = "test-agent",
                Timestamp = DateTime.UtcNow
            };

            _playerRepositoryMock.Setup(x => x.GetByPlayerIdAsync(command.PlayerId))
                .ReturnsAsync((Player)null);

            // Act
            await _handler.Handle(command);

            // Assert
            _pixelEventRepositoryMock.Verify(
                x => x.AddAsync(It.Is<PixelEvent>(e =>
                    e.EventType == command.EventType &&
                    e.PlayerId == command.PlayerId &&
                    e.BannerTag == command.BannerTag &&
                    e.SourceIp == command.SourceIp &&
                    e.UserAgent == command.UserAgent &&
                    e.Timestamp == command.Timestamp)),
                Times.Once);

            _playerRepositoryMock.Verify(
                x => x.AddAsync(It.Is<Player>(p =>
                    p.PlayerId == command.PlayerId)),
                Times.Once);
        }

        [Test]
        public async Task Handle_ExistingPlayer_UpdatesLastEvent()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create(command.PlayerId);
            _playerRepositoryMock.Setup(x => x.GetByPlayerIdAsync(command.PlayerId))
                .ReturnsAsync(existingPlayer);

            // Act
            await _handler.Handle(command);

            // Assert
            _playerRepositoryMock.Verify(
                x => x.UpdateAsync(It.Is<Player>(p =>
                    p.PlayerId == command.PlayerId &&
                    p.LastEventAt == command.Timestamp)),
                Times.Once);
        }

        [Test]
        public async Task Handle_RegistrationEvent_RegistersPlayer()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "registration",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create(command.PlayerId);
            _playerRepositoryMock.Setup(x => x.GetByPlayerIdAsync(command.PlayerId))
                .ReturnsAsync(existingPlayer);

            // Act
            await _handler.Handle(command);

            // Assert
            _playerRepositoryMock.Verify(
                x => x.UpdateAsync(It.Is<Player>(p =>
                    p.PlayerId == command.PlayerId &&
                    p.RegistrationDate.HasValue)),
                Times.Once);
        }

        [Test]
        public async Task Handle_DepositEvent_RecordsDeposit()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "deposit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create(command.PlayerId);
            existingPlayer.Register(); // Player must be registered before depositing
            _playerRepositoryMock.Setup(x => x.GetByPlayerIdAsync(command.PlayerId))
                .ReturnsAsync(existingPlayer);

            // Act
            await _handler.Handle(command);

            // Assert
            _playerRepositoryMock.Verify(
                x => x.UpdateAsync(It.Is<Player>(p =>
                    p.PlayerId == command.PlayerId &&
                    p.FirstDepositDate.HasValue)),
                Times.Once);
        }
    }
}
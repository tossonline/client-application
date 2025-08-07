using System;
using System.Collections.Generic;
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
        private Mock<IPixelEventRepository> _mockPixelEventRepository;
        private Mock<IPlayerRepository> _mockPlayerRepository;
        private Mock<ILogger<IngestPixelEventHandler>> _mockLogger;
        private IngestPixelEventHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockPixelEventRepository = new Mock<IPixelEventRepository>();
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _mockLogger = new Mock<ILogger<IngestPixelEventHandler>>();
            _handler = new IngestPixelEventHandler(_mockPixelEventRepository.Object, _mockPlayerRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Handle_WithValidCommand_ShouldSucceed()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "visit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Metadata = new Dictionary<string, string> { { "s", "bfp123456" } },
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0",
                Timestamp = DateTime.UtcNow
            };

            _mockPlayerRepository.Setup(x => x.GetByPlayerIdAsync("player123"))
                .ReturnsAsync((Player)null);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockPixelEventRepository.Verify(x => x.AddAsync(It.IsAny<PixelEvent>()), Times.Once);
            _mockPlayerRepository.Verify(x => x.AddAsync(It.IsAny<Player>()), Times.Once);
        }

        [Test]
        public async Task Handle_WithRegistrationEvent_ShouldRegisterPlayer()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "registration",
                PlayerId = "player123",
                BannerTag = "brandA",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create("player123");
            _mockPlayerRepository.Setup(x => x.GetByPlayerIdAsync("player123"))
                .ReturnsAsync(existingPlayer);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockPlayerRepository.Verify(x => x.UpdateAsync(It.Is<Player>(p => p.RegistrationDate.HasValue)), Times.Once);
        }

        [Test]
        public async Task Handle_WithDepositEvent_ShouldSetFirstDepositDate()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "deposit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create("player123");
            existingPlayer.Register();
            _mockPlayerRepository.Setup(x => x.GetByPlayerIdAsync("player123"))
                .ReturnsAsync(existingPlayer);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockPlayerRepository.Verify(x => x.UpdateAsync(It.Is<Player>(p => p.FirstDepositDate.HasValue)), Times.Once);
        }

        [Test]
        public void Handle_WithNullCommand_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null));
            Assert.That(ex.ParamName, Is.EqualTo("command"));
        }

        [Test]
        public async Task Handle_WithExistingPlayer_ShouldUpdatePlayer()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "visit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create("player123");
            _mockPlayerRepository.Setup(x => x.GetByPlayerIdAsync("player123"))
                .ReturnsAsync(existingPlayer);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockPlayerRepository.Verify(x => x.AddAsync(It.IsAny<Player>()), Times.Never);
            _mockPlayerRepository.Verify(x => x.UpdateAsync(It.IsAny<Player>()), Times.Once);
        }

        [Test]
        public async Task Handle_WithDepositEventWithoutRegistration_ShouldThrowException()
        {
            // Arrange
            var command = new IngestPixelEventCommand
            {
                EventType = "deposit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Timestamp = DateTime.UtcNow
            };

            var existingPlayer = Player.Create("player123");
            _mockPlayerRepository.Setup(x => x.GetByPlayerIdAsync("player123"))
                .ReturnsAsync(existingPlayer);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command));
            Assert.That(ex.Message, Is.EqualTo("Player must be registered before depositing"));
        }
    }
}


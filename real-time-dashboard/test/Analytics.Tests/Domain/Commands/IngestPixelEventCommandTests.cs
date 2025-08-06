using System;
using System.Collections.Generic;
using Analytics.Domain.Commands;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Commands
{
    [TestFixture]
    public class IngestPixelEventCommandTests
    {
        [Test]
        public void IngestPixelEventCommand_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var eventType = "visit";
            var playerId = "player123";
            var bannerTag = "brandA";

            // Act
            var command = new IngestPixelEventCommand(eventType, playerId, bannerTag);

            // Assert
            Assert.That(command.EventType, Is.EqualTo(eventType));
            Assert.That(command.PlayerId, Is.EqualTo(playerId));
            Assert.That(command.BannerTag, Is.EqualTo(bannerTag));
            Assert.That(command.Metadata, Is.Not.Null);
            Assert.That(command.Timestamp, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Test]
        public void IngestPixelEventCommand_Creation_WithMetadata_ShouldPreserveData()
        {
            // Arrange
            var metadata = new Dictionary<string, string>
            {
                { "s", "bfp123456" },
                { "b", "clickID126362362" }
            };

            // Act
            var command = new IngestPixelEventCommand("visit", "player123", "brandA")
            {
                Metadata = metadata,
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0"
            };

            // Assert
            Assert.That(command.Metadata.Count, Is.EqualTo(2));
            Assert.That(command.Metadata["s"], Is.EqualTo("bfp123456"));
            Assert.That(command.Metadata["b"], Is.EqualTo("clickID126362362"));
            Assert.That(command.SourceIp, Is.EqualTo("192.168.1.1"));
            Assert.That(command.UserAgent, Is.EqualTo("Mozilla/5.0"));
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var command = new IngestPixelEventCommand("visit", "player123", "brandA");

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithEmptyEventType_ShouldReturnFalse()
        {
            // Arrange
            var command = new IngestPixelEventCommand("", "player123", "brandA");

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithNullEventType_ShouldReturnFalse()
        {
            // Arrange
            var command = new IngestPixelEventCommand(null, "player123", "brandA");

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithEmptyPlayerId_ShouldReturnFalse()
        {
            // Arrange
            var command = new IngestPixelEventCommand("visit", "", "brandA");

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithEmptyBannerTag_ShouldReturnFalse()
        {
            // Arrange
            var command = new IngestPixelEventCommand("visit", "player123", "");

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithFutureTimestamp_ShouldReturnFalse()
        {
            // Arrange
            var command = new IngestPixelEventCommand("visit", "player123", "brandA")
            {
                Timestamp = DateTime.UtcNow.AddMinutes(10)
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithOldTimestamp_ShouldReturnFalse()
        {
            // Arrange
            var command = new IngestPixelEventCommand("visit", "player123", "brandA")
            {
                Timestamp = DateTime.UtcNow.AddDays(-2)
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IngestPixelEventCommand_Validation_WithValidTimestamp_ShouldReturnTrue()
        {
            // Arrange
            var command = new IngestPixelEventCommand("visit", "player123", "brandA")
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-2) // Within 5 minute window
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void IngestPixelEventCommand_WithDifferentEventTypes_ShouldValidateCorrectly()
        {
            // Arrange
            var visitCommand = new IngestPixelEventCommand("visit", "player123", "brandA");
            var registrationCommand = new IngestPixelEventCommand("registration", "player123", "brandA");
            var depositCommand = new IngestPixelEventCommand("deposit", "player123", "brandA");

            // Act & Assert
            Assert.That(visitCommand.IsValid(), Is.True);
            Assert.That(registrationCommand.IsValid(), Is.True);
            Assert.That(depositCommand.IsValid(), Is.True);
        }

        [Test]
        public void IngestPixelEventCommand_Serialization_ShouldPreserveAllProperties()
        {
            // Arrange
            var originalCommand = new IngestPixelEventCommand("visit", "player123", "brandA")
            {
                Metadata = new Dictionary<string, string> { { "s", "bfp123456" } },
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0",
                Timestamp = DateTime.UtcNow
            };

            // Act - Simulate serialization/deserialization
            var serializedCommand = new IngestPixelEventCommand
            {
                EventType = originalCommand.EventType,
                PlayerId = originalCommand.PlayerId,
                BannerTag = originalCommand.BannerTag,
                Metadata = new Dictionary<string, string>(originalCommand.Metadata),
                SourceIp = originalCommand.SourceIp,
                UserAgent = originalCommand.UserAgent,
                Timestamp = originalCommand.Timestamp
            };

            // Assert
            Assert.That(serializedCommand.EventType, Is.EqualTo(originalCommand.EventType));
            Assert.That(serializedCommand.PlayerId, Is.EqualTo(originalCommand.PlayerId));
            Assert.That(serializedCommand.BannerTag, Is.EqualTo(originalCommand.BannerTag));
            Assert.That(serializedCommand.Metadata, Is.EqualTo(originalCommand.Metadata));
            Assert.That(serializedCommand.SourceIp, Is.EqualTo(originalCommand.SourceIp));
            Assert.That(serializedCommand.UserAgent, Is.EqualTo(originalCommand.UserAgent));
            Assert.That(serializedCommand.Timestamp, Is.EqualTo(originalCommand.Timestamp));
        }
    }
} 
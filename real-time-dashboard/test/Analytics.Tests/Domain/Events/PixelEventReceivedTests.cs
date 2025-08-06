using System;
using System.Collections.Generic;
using Analytics.Domain.Events;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Events
{
    [TestFixture]
    public class PixelEventReceivedTests
    {
        [Test]
        public void PixelEventReceived_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var eventType = "visit";
            var playerId = "player123";
            var bannerTag = "brandA";
            var metadata = new Dictionary<string, string> { { "s", "bfp123456" } };

            // Act
            var @event = new PixelEventReceived
            {
                EventType = eventType,
                PlayerId = playerId,
                BannerTag = bannerTag,
                Metadata = metadata,
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0",
                Timestamp = DateTime.UtcNow
            };

            // Assert
            Assert.That(@event.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(@event.EventType, Is.EqualTo(eventType));
            Assert.That(@event.PlayerId, Is.EqualTo(playerId));
            Assert.That(@event.BannerTag, Is.EqualTo(bannerTag));
            Assert.That(@event.Metadata, Is.EqualTo(metadata));
            Assert.That(@event.SourceIp, Is.EqualTo("192.168.1.1"));
            Assert.That(@event.UserAgent, Is.EqualTo("Mozilla/5.0"));
            Assert.That(@event.ReceivedAt, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Test]
        public void PixelEventReceived_Creation_ShouldGenerateUniqueId()
        {
            // Act
            var event1 = new PixelEventReceived();
            var event2 = new PixelEventReceived();

            // Assert
            Assert.That(event1.Id, Is.Not.EqualTo(event2.Id));
            Assert.That(event1.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(event2.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void PixelEventReceived_Creation_ShouldSetReceivedAtToUtcNow()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var @event = new PixelEventReceived();

            // Assert
            Assert.That(@event.ReceivedAt, Is.GreaterThanOrEqualTo(beforeCreation));
            Assert.That(@event.ReceivedAt, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }

        [Test]
        public void PixelEventReceived_Creation_ShouldInitializeEmptyMetadata()
        {
            // Act
            var @event = new PixelEventReceived();

            // Assert
            Assert.That(@event.Metadata, Is.Not.Null);
            Assert.That(@event.Metadata.Count, Is.EqualTo(0));
        }

        [Test]
        public void PixelEventReceived_Serialization_ShouldPreserveAllProperties()
        {
            // Arrange
            var originalEvent = new PixelEventReceived
            {
                EventType = "visit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Metadata = new Dictionary<string, string> { { "s", "bfp123456" } },
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0",
                Timestamp = DateTime.UtcNow
            };

            // Act - Simulate serialization/deserialization
            var serializedEvent = new PixelEventReceived
            {
                Id = originalEvent.Id,
                EventType = originalEvent.EventType,
                PlayerId = originalEvent.PlayerId,
                BannerTag = originalEvent.BannerTag,
                Metadata = new Dictionary<string, string>(originalEvent.Metadata),
                SourceIp = originalEvent.SourceIp,
                UserAgent = originalEvent.UserAgent,
                Timestamp = originalEvent.Timestamp,
                ReceivedAt = originalEvent.ReceivedAt
            };

            // Assert
            Assert.That(serializedEvent.Id, Is.EqualTo(originalEvent.Id));
            Assert.That(serializedEvent.EventType, Is.EqualTo(originalEvent.EventType));
            Assert.That(serializedEvent.PlayerId, Is.EqualTo(originalEvent.PlayerId));
            Assert.That(serializedEvent.BannerTag, Is.EqualTo(originalEvent.BannerTag));
            Assert.That(serializedEvent.Metadata, Is.EqualTo(originalEvent.Metadata));
            Assert.That(serializedEvent.SourceIp, Is.EqualTo(originalEvent.SourceIp));
            Assert.That(serializedEvent.UserAgent, Is.EqualTo(originalEvent.UserAgent));
            Assert.That(serializedEvent.Timestamp, Is.EqualTo(originalEvent.Timestamp));
            Assert.That(serializedEvent.ReceivedAt, Is.EqualTo(originalEvent.ReceivedAt));
        }

        [Test]
        public void PixelEventReceived_MetadataPreservation_WithComplexData_ShouldSucceed()
        {
            // Arrange
            var metadata = new Dictionary<string, string>
            {
                { "s", "bfp123456" },
                { "b", "clickID126362362" },
                { "custom", "value" },
                { "timestamp", DateTime.UtcNow.ToString("O") }
            };

            // Act
            var @event = new PixelEventReceived
            {
                EventType = "visit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Metadata = metadata
            };

            // Assert
            Assert.That(@event.Metadata.Count, Is.EqualTo(4));
            Assert.That(@event.Metadata["s"], Is.EqualTo("bfp123456"));
            Assert.That(@event.Metadata["b"], Is.EqualTo("clickID126362362"));
            Assert.That(@event.Metadata["custom"], Is.EqualTo("value"));
        }

        [Test]
        public void PixelEventReceived_TimestampAccuracy_ShouldBeAccurate()
        {
            // Arrange
            var expectedTimestamp = DateTime.UtcNow;

            // Act
            var @event = new PixelEventReceived
            {
                EventType = "visit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Timestamp = expectedTimestamp
            };

            // Assert
            Assert.That(@event.Timestamp, Is.EqualTo(expectedTimestamp));
        }
    }
} 
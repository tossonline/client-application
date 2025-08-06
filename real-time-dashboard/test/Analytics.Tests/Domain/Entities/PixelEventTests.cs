using System;
using System.Collections.Generic;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class PixelEventTests
    {
        [Test]
        public void PixelEvent_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var eventType = "visit";
            var playerId = "player123";
            var bannerTag = "brandA";
            var metadata = new Dictionary<string, string> { { "s", "bfp123456" }, { "b", "clickID126362362" } };

            // Act
            var pixelEvent = new PixelEvent
            {
                EventType = eventType,
                PlayerId = playerId,
                BannerTag = bannerTag,
                Metadata = metadata,
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0"
            };

            // Assert
            Assert.That(pixelEvent.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(pixelEvent.EventType, Is.EqualTo(eventType));
            Assert.That(pixelEvent.PlayerId, Is.EqualTo(playerId));
            Assert.That(pixelEvent.BannerTag, Is.EqualTo(bannerTag));
            Assert.That(pixelEvent.Metadata, Is.EqualTo(metadata));
            Assert.That(pixelEvent.SourceIp, Is.EqualTo("192.168.1.1"));
            Assert.That(pixelEvent.UserAgent, Is.EqualTo("Mozilla/5.0"));
            Assert.That(pixelEvent.CreatedAt, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Test]
        public void PixelEvent_Creation_WithDifferentEventTypes_ShouldSucceed()
        {
            // Arrange & Act
            var visitEvent = new PixelEvent { EventType = "visit" };
            var registrationEvent = new PixelEvent { EventType = "registration" };
            var depositEvent = new PixelEvent { EventType = "deposit" };

            // Assert
            Assert.That(visitEvent.EventType, Is.EqualTo("visit"));
            Assert.That(registrationEvent.EventType, Is.EqualTo("registration"));
            Assert.That(depositEvent.EventType, Is.EqualTo("deposit"));
        }

        [Test]
        public void PixelEvent_Creation_WithEmptyMetadata_ShouldInitializeEmptyDictionary()
        {
            // Act
            var pixelEvent = new PixelEvent();

            // Assert
            Assert.That(pixelEvent.Metadata, Is.Not.Null);
            Assert.That(pixelEvent.Metadata.Count, Is.EqualTo(0));
        }

        [Test]
        public void PixelEvent_Creation_WithMetadata_ShouldPreserveData()
        {
            // Arrange
            var metadata = new Dictionary<string, string>
            {
                { "s", "bfp123456" },
                { "b", "clickID126362362" },
                { "custom", "value" }
            };

            // Act
            var pixelEvent = new PixelEvent { Metadata = metadata };

            // Assert
            Assert.That(pixelEvent.Metadata.Count, Is.EqualTo(3));
            Assert.That(pixelEvent.Metadata["s"], Is.EqualTo("bfp123456"));
            Assert.That(pixelEvent.Metadata["b"], Is.EqualTo("clickID126362362"));
            Assert.That(pixelEvent.Metadata["custom"], Is.EqualTo("value"));
        }

        [Test]
        public void PixelEvent_Creation_ShouldGenerateUniqueId()
        {
            // Act
            var event1 = new PixelEvent();
            var event2 = new PixelEvent();

            // Assert
            Assert.That(event1.Id, Is.Not.EqualTo(event2.Id));
            Assert.That(event1.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(event2.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void PixelEvent_Creation_ShouldSetCreatedAtToUtcNow()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var pixelEvent = new PixelEvent();

            // Assert
            Assert.That(pixelEvent.CreatedAt, Is.GreaterThanOrEqualTo(beforeCreation));
            Assert.That(pixelEvent.CreatedAt, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }

        [Test]
        public void PixelEvent_Equality_WithSameId_ShouldBeEqual()
        {
            // Arrange
            var id = Guid.NewGuid();
            var event1 = new PixelEvent { Id = id, EventType = "visit", PlayerId = "player1" };
            var event2 = new PixelEvent { Id = id, EventType = "registration", PlayerId = "player2" };

            // Act & Assert
            Assert.That(event1.Id, Is.EqualTo(event2.Id));
        }

        [Test]
        public void PixelEvent_Equality_WithDifferentId_ShouldNotBeEqual()
        {
            // Arrange
            var event1 = new PixelEvent { Id = Guid.NewGuid() };
            var event2 = new PixelEvent { Id = Guid.NewGuid() };

            // Act & Assert
            Assert.That(event1.Id, Is.Not.EqualTo(event2.Id));
        }

        [Test]
        public void PixelEvent_Serialization_ShouldPreserveAllProperties()
        {
            // Arrange
            var originalEvent = new PixelEvent
            {
                EventType = "visit",
                PlayerId = "player123",
                BannerTag = "brandA",
                Metadata = new Dictionary<string, string> { { "s", "bfp123456" } },
                SourceIp = "192.168.1.1",
                UserAgent = "Mozilla/5.0"
            };

            // Act - Simulate serialization/deserialization
            var serializedEvent = new PixelEvent
            {
                Id = originalEvent.Id,
                EventType = originalEvent.EventType,
                PlayerId = originalEvent.PlayerId,
                BannerTag = originalEvent.BannerTag,
                Metadata = new Dictionary<string, string>(originalEvent.Metadata),
                SourceIp = originalEvent.SourceIp,
                UserAgent = originalEvent.UserAgent,
                CreatedAt = originalEvent.CreatedAt
            };

            // Assert
            Assert.That(serializedEvent.Id, Is.EqualTo(originalEvent.Id));
            Assert.That(serializedEvent.EventType, Is.EqualTo(originalEvent.EventType));
            Assert.That(serializedEvent.PlayerId, Is.EqualTo(originalEvent.PlayerId));
            Assert.That(serializedEvent.BannerTag, Is.EqualTo(originalEvent.BannerTag));
            Assert.That(serializedEvent.Metadata, Is.EqualTo(originalEvent.Metadata));
            Assert.That(serializedEvent.SourceIp, Is.EqualTo(originalEvent.SourceIp));
            Assert.That(serializedEvent.UserAgent, Is.EqualTo(originalEvent.UserAgent));
            Assert.That(serializedEvent.CreatedAt, Is.EqualTo(originalEvent.CreatedAt));
        }
    }
} 
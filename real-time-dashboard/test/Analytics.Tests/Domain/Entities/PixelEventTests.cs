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
        public void PixelEvent_WhenCreated_HasExpectedDefaults()
        {
            // Arrange & Act
            var pixelEvent = new PixelEvent();

            // Assert
            Assert.That(pixelEvent.EventType, Is.EqualTo(string.Empty));
            Assert.That(pixelEvent.PlayerId, Is.EqualTo(string.Empty));
            Assert.That(pixelEvent.BannerTag, Is.EqualTo(string.Empty));
            Assert.That(pixelEvent.Metadata, Is.Not.Null);
            Assert.That(pixelEvent.Metadata, Is.Empty);
            Assert.That(pixelEvent.SourceIp, Is.Null);
            Assert.That(pixelEvent.UserAgent, Is.Null);
        }

        [Test]
        public void PixelEvent_WithValues_SetsPropertiesCorrectly()
        {
            // Arrange
            var timestamp = DateTime.UtcNow;
            var metadata = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            var pixelEvent = new PixelEvent
            {
                EventType = "visit",
                PlayerId = "player-123",
                BannerTag = "banner-456",
                Metadata = metadata,
                SourceIp = "127.0.0.1",
                UserAgent = "test-agent",
                Timestamp = timestamp
            };

            // Assert
            Assert.That(pixelEvent.EventType, Is.EqualTo("visit"));
            Assert.That(pixelEvent.PlayerId, Is.EqualTo("player-123"));
            Assert.That(pixelEvent.BannerTag, Is.EqualTo("banner-456"));
            Assert.That(pixelEvent.Metadata, Is.EqualTo(metadata));
            Assert.That(pixelEvent.SourceIp, Is.EqualTo("127.0.0.1"));
            Assert.That(pixelEvent.UserAgent, Is.EqualTo("test-agent"));
            Assert.That(pixelEvent.Timestamp, Is.EqualTo(timestamp));
        }

        [Test]
        public void PixelEvent_MetadataIsModifiable()
        {
            // Arrange
            var pixelEvent = new PixelEvent();

            // Act
            pixelEvent.Metadata["key1"] = "value1";
            pixelEvent.Metadata["key2"] = "value2";

            // Assert
            Assert.That(pixelEvent.Metadata.Count, Is.EqualTo(2));
            Assert.That(pixelEvent.Metadata["key1"], Is.EqualTo("value1"));
            Assert.That(pixelEvent.Metadata["key2"], Is.EqualTo("value2"));
        }
    }
}
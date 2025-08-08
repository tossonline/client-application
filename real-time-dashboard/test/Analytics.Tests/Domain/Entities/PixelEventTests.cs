using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class PixelEventTests
    {
        private const string ValidPlayerId = "player123";
        private const string ValidBannerTag = "campaign1-banner-300x250";

        [Test]
        public void CreateVisit_WithValidData_CreatesEvent()
        {
            // Act
            var pixelEvent = PixelEvent.CreateVisit(ValidPlayerId, ValidBannerTag);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(pixelEvent.EventType, Is.EqualTo("visit"));
                Assert.That(pixelEvent.PlayerId, Is.EqualTo(ValidPlayerId));
                Assert.That(pixelEvent.BannerTag, Is.EqualTo(ValidBannerTag));
                Assert.That(pixelEvent.CampaignId, Is.EqualTo("campaign1"));
                Assert.That(pixelEvent.Timestamp, Is.LessThanOrEqualTo(DateTime.UtcNow));
            });
        }

        [Test]
        public void CreateRegistration_WithValidData_CreatesEvent()
        {
            // Act
            var pixelEvent = PixelEvent.CreateRegistration(ValidPlayerId, ValidBannerTag);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(pixelEvent.EventType, Is.EqualTo("registration"));
                Assert.That(pixelEvent.PlayerId, Is.EqualTo(ValidPlayerId));
                Assert.That(pixelEvent.BannerTag, Is.EqualTo(ValidBannerTag));
            });
        }

        [Test]
        public void CreateDeposit_WithValidData_CreatesEvent()
        {
            // Act
            var pixelEvent = PixelEvent.CreateDeposit(ValidPlayerId, ValidBannerTag);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(pixelEvent.EventType, Is.EqualTo("deposit"));
                Assert.That(pixelEvent.PlayerId, Is.EqualTo(ValidPlayerId));
                Assert.That(pixelEvent.BannerTag, Is.EqualTo(ValidBannerTag));
            });
        }

        [Test]
        public void Create_WithNullPlayerId_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                PixelEvent.CreateVisit(null, ValidBannerTag));
            Assert.That(ex.Message, Does.Contain("Player ID cannot be null"));
        }

        [Test]
        public void Create_WithEmptyBannerTag_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                PixelEvent.CreateVisit(ValidPlayerId, string.Empty));
            Assert.That(ex.Message, Does.Contain("Banner tag cannot be null"));
        }

        [Test]
        public void AddMetadata_WithValidData_AddsToMetadata()
        {
            // Arrange
            var pixelEvent = PixelEvent.CreateVisit(ValidPlayerId, ValidBannerTag);

            // Act
            pixelEvent.AddMetadata("browser", "chrome");
            pixelEvent.AddMetadata("os", "windows");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(pixelEvent.Metadata, Does.ContainKey("browser"));
                Assert.That(pixelEvent.Metadata["browser"], Is.EqualTo("chrome"));
                Assert.That(pixelEvent.Metadata, Does.ContainKey("os"));
                Assert.That(pixelEvent.Metadata["os"], Is.EqualTo("windows"));
            });
        }

        [Test]
        public void AddMetadata_WithNullKey_ThrowsArgumentException()
        {
            // Arrange
            var pixelEvent = PixelEvent.CreateVisit(ValidPlayerId, ValidBannerTag);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                pixelEvent.AddMetadata(null, "value"));
            Assert.That(ex.Message, Does.Contain("key cannot be null"));
        }

        [Test]
        public void IsValid_WithValidEvent_ReturnsTrue()
        {
            // Arrange
            var pixelEvent = PixelEvent.CreateVisit(ValidPlayerId, ValidBannerTag);

            // Act & Assert
            Assert.That(pixelEvent.IsValid(), Is.True);
        }

        [TestCase("campaign1-banner-300x250", "campaign1")]
        [TestCase("campaign2", "campaign2")]
        [TestCase("campaign3-other", "campaign3")]
        public void ExtractCampaignId_ExtractsCorrectly(string bannerTag, string expectedCampaignId)
        {
            // Arrange & Act
            var pixelEvent = PixelEvent.CreateVisit(ValidPlayerId, bannerTag);

            // Assert
            Assert.That(pixelEvent.CampaignId, Is.EqualTo(expectedCampaignId));
        }

        [Test]
        public void Create_WithSourceIpAndUserAgent_SetsProperties()
        {
            // Arrange
            const string sourceIp = "192.168.1.1";
            const string userAgent = "Mozilla/5.0";

            // Act
            var pixelEvent = PixelEvent.CreateVisit(ValidPlayerId, ValidBannerTag, sourceIp, userAgent);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(pixelEvent.SourceIp, Is.EqualTo(sourceIp));
                Assert.That(pixelEvent.UserAgent, Is.EqualTo(userAgent));
            });
        }
    }
}
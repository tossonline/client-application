using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class DailyMetricTests
    {
        private DateTime _testDate;
        private const string EventType = "visit";

        [SetUp]
        public void Setup()
        {
            _testDate = DateTime.UtcNow.Date;
        }

        [Test]
        public void Create_WithValidData_CreatesMetric()
        {
            // Act
            var metric = DailyMetric.Create(_testDate, EventType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metric.Date, Is.EqualTo(_testDate));
                Assert.That(metric.EventType, Is.EqualTo(EventType));
                Assert.That(metric.VisitCount, Is.EqualTo(0));
                Assert.That(metric.RegistrationCount, Is.EqualTo(0));
                Assert.That(metric.DepositCount, Is.EqualTo(0));
                Assert.That(metric.ConversionRate, Is.EqualTo(0));
                Assert.That(metric.DepositRate, Is.EqualTo(0));
                Assert.That(metric.Trend, Is.EqualTo(TrendIndicator.Stable));
            });
        }

        [Test]
        public void Create_WithNullEventType_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                DailyMetric.Create(_testDate, null));
            Assert.That(ex.Message, Does.Contain("Event type cannot be null"));
        }

        [Test]
        public void UpdateVisitCount_UpdatesCountAndRates()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);

            // Act
            metric.UpdateVisitCount(100);
            metric.UpdateRegistrationCount(20);
            metric.UpdateDepositCount(10);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(metric.VisitCount, Is.EqualTo(100));
                Assert.That(metric.ConversionRate, Is.EqualTo(20)); // 20%
                Assert.That(metric.DepositRate, Is.EqualTo(50));    // 50%
            });
        }

        [Test]
        public void UpdateVisitCount_WithNegativeCount_ThrowsArgumentException()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => metric.UpdateVisitCount(-1));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }

        [Test]
        public void UpdateRegistrationCount_UpdatesCountAndRates()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);
            metric.UpdateVisitCount(200);

            // Act
            metric.UpdateRegistrationCount(40);

            // Assert
            Assert.That(metric.ConversionRate, Is.EqualTo(20)); // 20%
        }

        [Test]
        public void UpdateRegistrationCount_WithNegativeCount_ThrowsArgumentException()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => metric.UpdateRegistrationCount(-1));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }

        [Test]
        public void UpdateDepositCount_UpdatesCountAndRates()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);
            metric.UpdateRegistrationCount(50);

            // Act
            metric.UpdateDepositCount(25);

            // Assert
            Assert.That(metric.DepositRate, Is.EqualTo(50)); // 50%
        }

        [Test]
        public void UpdateDepositCount_WithNegativeCount_ThrowsArgumentException()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => metric.UpdateDepositCount(-1));
            Assert.That(ex.Message, Does.Contain("cannot be negative"));
        }

        [Test]
        public void UpdateTrend_WithHigherConversionRate_SetsIncreasing()
        {
            // Arrange
            var previousMetric = DailyMetric.Create(_testDate.AddDays(-1), EventType);
            var currentMetric = DailyMetric.Create(_testDate, EventType);

            previousMetric.UpdateVisitCount(100);
            previousMetric.UpdateRegistrationCount(10); // 10% conversion

            currentMetric.UpdateVisitCount(100);
            currentMetric.UpdateRegistrationCount(20); // 20% conversion

            // Act
            currentMetric.UpdateTrend(previousMetric);

            // Assert
            Assert.That(currentMetric.Trend, Is.EqualTo(TrendIndicator.Increasing));
        }

        [Test]
        public void UpdateTrend_WithLowerConversionRate_SetsDecreasing()
        {
            // Arrange
            var previousMetric = DailyMetric.Create(_testDate.AddDays(-1), EventType);
            var currentMetric = DailyMetric.Create(_testDate, EventType);

            previousMetric.UpdateVisitCount(100);
            previousMetric.UpdateRegistrationCount(20); // 20% conversion

            currentMetric.UpdateVisitCount(100);
            currentMetric.UpdateRegistrationCount(5);  // 5% conversion

            // Act
            currentMetric.UpdateTrend(previousMetric);

            // Assert
            Assert.That(currentMetric.Trend, Is.EqualTo(TrendIndicator.Decreasing));
        }

        [Test]
        public void UpdateTrend_WithSimilarConversionRate_SetsStable()
        {
            // Arrange
            var previousMetric = DailyMetric.Create(_testDate.AddDays(-1), EventType);
            var currentMetric = DailyMetric.Create(_testDate, EventType);

            previousMetric.UpdateVisitCount(100);
            previousMetric.UpdateRegistrationCount(10); // 10% conversion

            currentMetric.UpdateVisitCount(100);
            currentMetric.UpdateRegistrationCount(10); // 10% conversion

            // Act
            currentMetric.UpdateTrend(previousMetric);

            // Assert
            Assert.That(currentMetric.Trend, Is.EqualTo(TrendIndicator.Stable));
        }

        [Test]
        public void UpdateTrend_WithNullPreviousDay_SetsStable()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);

            // Act
            metric.UpdateTrend(null);

            // Assert
            Assert.That(metric.Trend, Is.EqualTo(TrendIndicator.Stable));
        }

        [Test]
        public void ConversionRate_WithZeroVisits_ReturnsZero()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);
            metric.UpdateRegistrationCount(10);

            // Assert
            Assert.That(metric.ConversionRate, Is.EqualTo(0));
        }

        [Test]
        public void DepositRate_WithZeroRegistrations_ReturnsZero()
        {
            // Arrange
            var metric = DailyMetric.Create(_testDate, EventType);
            metric.UpdateDepositCount(5);

            // Assert
            Assert.That(metric.DepositRate, Is.EqualTo(0));
        }
    }
}
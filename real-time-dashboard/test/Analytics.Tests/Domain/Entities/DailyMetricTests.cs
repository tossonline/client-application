using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class DailyMetricTests
    {
        [Test]
        public void DailyMetric_WhenCreated_HasExpectedDefaults()
        {
            // Arrange & Act
            var metric = new DailyMetric();

            // Assert
            Assert.That(metric.EventType, Is.EqualTo(string.Empty));
            Assert.That(metric.VisitCount, Is.EqualTo(0));
            Assert.That(metric.RegistrationCount, Is.EqualTo(0));
            Assert.That(metric.DepositCount, Is.EqualTo(0));
            Assert.That(metric.ConversionRate, Is.EqualTo(0));
        }

        [Test]
        public void DailyMetric_WithValues_SetsPropertiesCorrectly()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;

            // Act
            var metric = new DailyMetric
            {
                Date = date,
                EventType = "visit",
                VisitCount = 100,
                RegistrationCount = 20,
                DepositCount = 10,
                ConversionRate = 20.0m
            };

            // Assert
            Assert.That(metric.Date, Is.EqualTo(date));
            Assert.That(metric.EventType, Is.EqualTo("visit"));
            Assert.That(metric.VisitCount, Is.EqualTo(100));
            Assert.That(metric.RegistrationCount, Is.EqualTo(20));
            Assert.That(metric.DepositCount, Is.EqualTo(10));
            Assert.That(metric.ConversionRate, Is.EqualTo(20.0m));
        }
    }
}

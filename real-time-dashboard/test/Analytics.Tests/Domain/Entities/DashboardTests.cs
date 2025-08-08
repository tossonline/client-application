using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class DashboardTests
    {
        private const string ValidName = "Test Dashboard";
        private const string ValidDescription = "Test Description";
        private const string ValidOwnerId = "user123";

        [Test]
        public void Create_WithValidData_CreatesDashboard()
        {
            // Act
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dashboard.Name, Is.EqualTo(ValidName));
                Assert.That(dashboard.Description, Is.EqualTo(ValidDescription));
                Assert.That(dashboard.Type, Is.EqualTo(DashboardType.Overview));
                Assert.That(dashboard.OwnerId, Is.EqualTo(ValidOwnerId));
                Assert.That(dashboard.CreatedAt, Is.LessThanOrEqualTo(DateTime.UtcNow));
                Assert.That(dashboard.IsActive, Is.True);
                Assert.That(dashboard.Layout, Is.Not.Null);
                Assert.That(dashboard.Configuration, Is.Empty);
            });
        }

        [Test]
        public void Create_WithNullName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Dashboard.Create(null, ValidDescription, DashboardType.Overview, ValidOwnerId));
            Assert.That(ex.Message, Does.Contain("Name cannot be null"));
        }

        [Test]
        public void Create_WithNullOwnerId_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, null));
            Assert.That(ex.Message, Does.Contain("Owner ID cannot be null"));
        }

        [Test]
        public void Update_UpdatesNameAndDescription()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);
            var newName = "New Name";
            var newDescription = "New Description";

            // Act
            dashboard.Update(newName, newDescription);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dashboard.Name, Is.EqualTo(newName));
                Assert.That(dashboard.Description, Is.EqualTo(newDescription));
                Assert.That(dashboard.UpdatedAt, Is.Not.Null);
            });
        }

        [Test]
        public void Update_WithNullName_ThrowsArgumentException()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => dashboard.Update(null, "New Description"));
            Assert.That(ex.Message, Does.Contain("Name cannot be null"));
        }

        [Test]
        public void Activate_ActivatesDashboard()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);
            dashboard.Deactivate();

            // Act
            dashboard.Activate();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dashboard.IsActive, Is.True);
                Assert.That(dashboard.UpdatedAt, Is.Not.Null);
            });
        }

        [Test]
        public void Deactivate_DeactivatesDashboard()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);

            // Act
            dashboard.Deactivate();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dashboard.IsActive, Is.False);
                Assert.That(dashboard.UpdatedAt, Is.Not.Null);
            });
        }

        [Test]
        public void UpdateLayout_UpdatesLayout()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);
            var layout = new DashboardLayout
            {
                GridLayout = "grid-template-columns: 1fr 1fr;",
                Widgets = new List<DashboardWidget>
                {
                    new DashboardWidget
                    {
                        Id = "widget1",
                        Type = "chart",
                        Title = "Test Widget",
                        Position = new GridPosition { Row = 1, Column = 1, Width = 1, Height = 1 }
                    }
                }
            };

            // Act
            dashboard.UpdateLayout(layout);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dashboard.Layout.GridLayout, Is.EqualTo(layout.GridLayout));
                Assert.That(dashboard.Layout.Widgets, Has.Count.EqualTo(1));
                Assert.That(dashboard.UpdatedAt, Is.Not.Null);
            });
        }

        [Test]
        public void UpdateLayout_WithNullLayout_ThrowsArgumentNullException()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => dashboard.UpdateLayout(null));
        }

        [Test]
        public void UpdateConfiguration_UpdatesConfiguration()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);

            // Act
            dashboard.UpdateConfiguration("refreshInterval", 30);
            dashboard.UpdateConfiguration("theme", "dark");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dashboard.Configuration, Does.ContainKey("refreshInterval"));
                Assert.That(dashboard.Configuration["refreshInterval"], Is.EqualTo(30));
                Assert.That(dashboard.Configuration, Does.ContainKey("theme"));
                Assert.That(dashboard.Configuration["theme"], Is.EqualTo("dark"));
                Assert.That(dashboard.UpdatedAt, Is.Not.Null);
            });
        }

        [Test]
        public void UpdateConfiguration_WithNullKey_ThrowsArgumentException()
        {
            // Arrange
            var dashboard = Dashboard.Create(ValidName, ValidDescription, DashboardType.Overview, ValidOwnerId);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => dashboard.UpdateConfiguration(null, "value"));
            Assert.That(ex.Message, Does.Contain("key cannot be null"));
        }

        [TestCase(DashboardType.Overview)]
        [TestCase(DashboardType.Campaign)]
        [TestCase(DashboardType.Conversion)]
        [TestCase(DashboardType.RealTime)]
        [TestCase(DashboardType.Custom)]
        public void Create_WithDifferentTypes_SetsDashboardType(DashboardType type)
        {
            // Act
            var dashboard = Dashboard.Create(ValidName, ValidDescription, type, ValidOwnerId);

            // Assert
            Assert.That(dashboard.Type, Is.EqualTo(type));
        }
    }
}

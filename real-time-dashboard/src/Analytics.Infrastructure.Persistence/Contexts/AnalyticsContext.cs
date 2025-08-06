// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Persistence.Mappings;

namespace Analytics.Infrastructure.Persistence.Contexts
{
    public class AnalyticsContext : DbContext
    {
        public AnalyticsContext()
        {
        }

        public AnalyticsContext(DbContextOptions<AnalyticsContext> options)
            : base(options)
        {
        }

        // Entity DbSets
        public DbSet<Dashboards> Dashboards { get; set; }
        public DbSet<PixelEvent> PixelEvents { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<EventSummary> EventSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new DashboardsConfiguration());
            modelBuilder.ApplyConfiguration(new PixelEventConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerConfiguration());
            modelBuilder.ApplyConfiguration(new EventSummaryConfiguration());

            // Configure database schema defaults
            modelBuilder.HasDefaultSchema("dbo");

            // Global query filters and configurations
            ConfigureGlobalSettings(modelBuilder);
            
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
        {
            // Set default datetime precision for all datetime columns
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("datetime2(7)");
                    }
                }
            }

            // Configure decimal precision for all decimal columns
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetColumnType("decimal(18,4)");
                    }
                }
            }
        }

        // Override SaveChanges to automatically update audit fields
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                var entityType = entity.GetType();

                // Update UpdatedAt field for modified entities
                if (entry.State == EntityState.Modified)
                {
                    var updatedAtProperty = entityType.GetProperty("UpdatedAt");
                    if (updatedAtProperty != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }

                // Set CreatedAt for new entities
                if (entry.State == EntityState.Added)
                {
                    var createdAtProperty = entityType.GetProperty("CreatedAt");
                    if (createdAtProperty != null)
                    {
                        entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    }

                    var updatedAtProperty = entityType.GetProperty("UpdatedAt");
                    if (updatedAtProperty != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
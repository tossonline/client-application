using Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Persistence.Contexts
{
    public class AnalyticsContext : DbContext
    {
        public AnalyticsContext(DbContextOptions<AnalyticsContext> options) : base(options)
        {
        }

        public DbSet<PixelEvent> PixelEvents { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<EventSummary> EventSummaries { get; set; }
        public DbSet<DailyMetric> DailyMetrics { get; set; }
        public DbSet<Dashboards> Dashboards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PixelEvent configuration
            modelBuilder.Entity<PixelEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PlayerId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BannerTag).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SourceIp).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.HasIndex(e => e.PlayerId);
                entity.HasIndex(e => e.EventType);
                entity.HasIndex(e => e.BannerTag);
                entity.HasIndex(e => e.Timestamp);
            });

            // Player configuration
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlayerId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstSeen).IsRequired();
                entity.HasIndex(e => e.PlayerId).IsUnique();
                entity.HasIndex(e => e.RegistrationDate);
                entity.HasIndex(e => e.FirstDepositDate);
            });

            // EventSummary configuration
            modelBuilder.Entity<EventSummary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BannerTag).HasMaxLength(100);
                entity.Property(e => e.EventDate).IsRequired();
                entity.HasIndex(e => new { e.EventDate, e.EventType, e.BannerTag });
            });

            // DailyMetric configuration
            modelBuilder.Entity<DailyMetric>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Date).IsRequired();
                entity.HasIndex(e => new { e.Date, e.EventType });
            });

            // Dashboards configuration
            modelBuilder.Entity<Dashboards>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }
    }
}
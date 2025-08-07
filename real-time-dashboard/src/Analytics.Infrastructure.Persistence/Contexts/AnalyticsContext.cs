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

        public DbSet<Dashboards> Dashboardss { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DashboardsConfiguration());
        }
    }
}
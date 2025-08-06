// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Analytics.Infrastructure.Persistence.Contexts;

namespace Analytics.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Design-time factory for creating AnalyticsContext instances during migrations
    /// </summary>
    public class AnalyticsContextFactory : IDesignTimeDbContextFactory<AnalyticsContext>
    {
        public AnalyticsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnalyticsContext>();
            
            // Use a default connection string for migrations
            // This should be overridden in actual deployment
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=dbAffiliate_Analytics;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
            
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);
                options.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            return new AnalyticsContext(optionsBuilder.Options);
        }
    }
}
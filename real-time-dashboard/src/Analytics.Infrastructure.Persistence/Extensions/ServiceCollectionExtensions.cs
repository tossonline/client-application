using Analytics.Domain.Repositories;
using Analytics.Infrastructure.Persistence.Contexts;
using Analytics.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            // Add DbContext
            services.AddDbContext<AnalyticsContext>(options =>
                options.UseNpgsql(connectionString));

            // Add repositories
            services.AddScoped<IPixelEventRepository, PixelEventRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IEventSummaryRepository, EventSummaryRepository>();
            services.AddScoped<IDashboardsRepository, DashboardsRepository>();

            return services;
        }
    }
}
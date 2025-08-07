using Analytics.Infrastructure.Service.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class WebSocketRegistration
    {
        public static IServiceCollection AddWebSocketSupport(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
                options.StreamBufferCapacity = 10;
            });

            return services;
        }

        public static IApplicationBuilder UseWebSocketSupport(this IApplicationBuilder app)
        {
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024 // 4KB
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AnalyticsHub>("/hubs/analytics");
            });

            return app;
        }
    }
}

// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Analytics.Infrastructure.Service
{
    internal class Program
    {
        protected Program() { }

        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Add configuration sources if needed
                })
                .ConfigureServices((context, services) =>
                {
                    // Register your services here
                    // Example: services.AddSingleton<IAnalyticsConfiguration, AnalyticsConfiguration>();
                    // Add other DI registrations as needed
                })
                .Build();

            host.Run();
        }
    }
}
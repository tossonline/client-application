// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Extensions.Service;
using Affiliate.Platform.Extensions.Service.Configuration;
using Affiliate.Platform.Extensions.Service.Configuration.Hooks;
using Analytics.Domain.Models.Configuration;
using Analytics.Infrastructure.Service.Dependencies;

namespace Analytics.Infrastructure.Service
{
    internal class Program
    {
        protected Program()
        {
        }

        protected static void Main()
        {
            ShellConfigurationManager<AnalyticsConfiguration> configurationManager = new();
            IAnalyticsConfiguration configuration = configurationManager.UseAppSettings();

            configuration.Hooks = new HookConfiguration
            {
                RegisterHealthchecks = HealthCheckRegistration.RegisterDependencies,
                RegisterDependencies = DependencyRegistration.RegisterDependencies,
                RegisterFeatures = FeatureRegistration.RegisterDependencies,
                RegisterControllers = ControllerRegistration.RegisterDependencies,
                RegisterMetrics = MetricsRegistration.RegisterDependencies,
                RegisterMiddleware = MiddlewareRegistration.RegisterDependencies
            };

            Shell shell = new(configuration);
            shell.Start();
        }
    }
}
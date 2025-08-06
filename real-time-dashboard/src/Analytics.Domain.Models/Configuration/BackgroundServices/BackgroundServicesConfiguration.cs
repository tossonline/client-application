// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.BackgroundServices
{
    [Serializable]
    public sealed class BackgroundServicesConfiguration
    {
        public BackgroundServicesConfiguration()
        {
            Dashboard = new DashboardConfiguration();
            JobConfigurations = new JobConfigurations();
        }

        public DashboardConfiguration Dashboard { get; init; }
        public JobConfigurations JobConfigurations { get; init; }
    }
}
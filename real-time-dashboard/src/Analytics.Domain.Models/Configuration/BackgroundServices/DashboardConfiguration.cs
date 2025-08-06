// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.BackgroundServices
{
    [Serializable]
    public sealed class DashboardConfiguration
    {
        public DashboardConfiguration()
        {
            PrefixPath = string.Empty;
            Path = string.Empty;
            Title = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
        }

        public string PrefixPath { get; init; }

        public string Path { get; init; }

        public string Title { get; init; }

        public string Username { get; init; }

        public string Password { get; init; }
    }
}
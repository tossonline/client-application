// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.Logger
{
    [Serializable]
    public sealed class LoggerOptionsConfiguration
    {
        public LoggerOptionsConfiguration()
        {
            Team = string.Empty;
            Environment = string.Empty;
            ApplicationType = string.Empty;
            Application = string.Empty;
            Source = string.Empty;
            Endpoint = string.Empty;
        }

        public string Team { get; set; }
        public string Environment { get; set; }
        public string ApplicationType { get; set; }
        public string Application { get; set; }
        public string Source { get; set; }
        public string Endpoint { get; set; }
    }
}
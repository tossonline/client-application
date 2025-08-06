// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.HealthCheck
{
    public sealed class SqlHealthCheck
    {
        public SqlHealthCheck(string name, string connectionString, string server, IEnumerable<string> labels)
        {
            Name = name;
            ConnectionString = connectionString;
            Server = server;
            Labels = labels;
        }

        public string Name { get; set; }

        public string ConnectionString { get; }

        public string Server { get; set; }

        public IEnumerable<string> Labels { get; }
    }
}
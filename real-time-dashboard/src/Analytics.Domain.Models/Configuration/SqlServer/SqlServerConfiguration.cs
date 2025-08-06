// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.SqlServer
{
    [Serializable]
    public class SqlServerConfiguration
    {
        public SqlServerConfiguration()
        {
            Server = string.Empty;
            Database = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Encrypt = false;
            AllowMultipleActiveResultSets = true;
        }

        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Encrypt { get; set; }
        public bool AllowMultipleActiveResultSets { get; set; }

        public string ConnectionString => $"Server={Server};Database={Database};User Id={Username};Password={Password};Encrypt={Encrypt};MultipleActiveResultSets={AllowMultipleActiveResultSets};Connect Timeout=60";
    }
}
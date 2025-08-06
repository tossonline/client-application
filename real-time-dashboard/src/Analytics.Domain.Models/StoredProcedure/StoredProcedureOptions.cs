// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.StoredProcedure
{
    public sealed class StoredProcedureOptions
    {
        public StoredProcedureOptions()
        {
            Procedure = string.Empty;
            ConnectionString = string.Empty;
            Parameters = null;
        }

        public string Procedure { get; init; }

        public string ConnectionString { get; init; }

        public object? Parameters { get; init; }
    }
}
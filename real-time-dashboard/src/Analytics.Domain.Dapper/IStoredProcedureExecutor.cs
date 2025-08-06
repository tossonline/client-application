// Copyright (c) DigiOutsource. All rights reserved.

using Analytics.Domain.Models.StoredProcedure;

namespace Analytics.Domain.Dapper
{
    public interface IStoredProcedureExecutor
    {
        Task Run(StoredProcedureOptions options);
    }
}
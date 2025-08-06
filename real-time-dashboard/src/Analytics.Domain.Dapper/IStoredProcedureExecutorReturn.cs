// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Dapper
{
    public interface IStoredProcedureExecutorReturn<TMessage>
    {
        Task<List<TMessage>> Get(string connectionString, string storedProcedure, object? parameters = null);
    }
}
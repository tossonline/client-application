// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Messaging.Abstractions.Messages;

namespace Analytics.Domain.Dapper
{
    public interface IStoredProcedureExecutorReturn<TMessage>
        where TMessage : Message
    {
        Task<List<TMessage>> Get(string connectionString, string storedProcedure, object? parameters = null);
    }
}
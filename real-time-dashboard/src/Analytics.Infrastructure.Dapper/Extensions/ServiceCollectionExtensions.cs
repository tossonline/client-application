// Copyright (c) DigiOutsource. All rights reserved.

using Affiliate.Platform.Messaging.Abstractions.Messages;
using Microsoft.Extensions.DependencyInjection;
using Analytics.Domain.Dapper;

namespace Analytics.Infrastructure.Dapper.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStoredProcedureExecutor(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IStoredProcedureExecutor, StoredProcedureExecutor>();
        }
        
        public static IServiceCollection AddStoredProcedureExecutor<TMessage>(this IServiceCollection serviceCollection)
            where TMessage : Message
        {
            return serviceCollection
                .AddSingleton<IStoredProcedureExecutorReturn<TMessage>, StoredProcedureExecutorReturn<TMessage>>();
        }
    }
}
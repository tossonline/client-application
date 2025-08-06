// Copyright (c) DigiOutsource. All rights reserved.

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
    }
}
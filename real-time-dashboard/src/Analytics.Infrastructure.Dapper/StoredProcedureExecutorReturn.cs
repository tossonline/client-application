// Copyright (c) DigiOutsource. All rights reserved.

using System.Data;
using System.Diagnostics;
using Affiliate.Platform.Messaging.Abstractions.Messages;
using Dapper;
using Microsoft.Data.SqlClient;
using Analytics.Domain.Dapper;
using Analytics.Domain.Observability.Messages;
using Analytics.Infrastructure.Dapper.Exceptions;

namespace Analytics.Infrastructure.Dapper
{
    public sealed class StoredProcedureExecutorReturn<TMessage> : IStoredProcedureExecutorReturn<TMessage>
        where TMessage : Message
    {
        private const int QUERY_TIMEOUT = 600;

        public async Task<List<TMessage>> Get(string connectionString, string storedProcedure, object? parameters = null)
        {
            using (SqlConnection connection = new(connectionString))
            {
                Stopwatch timer = new();
                timer.Start();
                
                try
                {
                    await connection
                        .OpenAsync();

                    return (await connection.QueryAsync<TMessage>(storedProcedure, parameters, null, QUERY_TIMEOUT, CommandType.StoredProcedure)).ToList();
                }
                catch (TimeoutException ex)
                {
                    timer.Stop();
                    throw new DapperException($"{ErrorMessages.DAPPER_TIMEOUT_ERROR} - Execution Time (Seconds) {timer.Elapsed.Seconds}", ex);
                }
                catch (SqlException ex)
                {
                    timer.Stop();
                    throw new DapperException($"{ErrorMessages.DAPPER_SQL_EXCEPTION_ERROR} - Execution Time (Seconds) {timer.Elapsed.Seconds}", ex);
                }
                catch (Exception ex)
                {
                    timer.Stop();
                    throw new DapperException($"{ErrorMessages.DAPPER_NON_SQL_EXCEPTION_ERROR} - Execution Time (Seconds) {timer.Elapsed.Seconds}", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
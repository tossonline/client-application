// Copyright (c) DigiOutsource. All rights reserved.

using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;
using Analytics.Domain.Dapper;
using Analytics.Domain.Models.StoredProcedure;
using Analytics.Domain.Observability.Messages;
using Analytics.Infrastructure.Dapper.Exceptions;

namespace Analytics.Infrastructure.Dapper
{
    public sealed class StoredProcedureExecutor : IStoredProcedureExecutor
    {
        private const int QUERY_TIMEOUT = 1800;

        public async Task Run(StoredProcedureOptions options)
        {
            using (SqlConnection connection = new(options.ConnectionString))
            {
                Stopwatch timer = new();
                timer.Start();

                try
                {
                    await connection
                        .OpenAsync();

                    await connection
                        .ExecuteAsync(options.Procedure, options.Parameters, null, QUERY_TIMEOUT, CommandType.StoredProcedure);
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
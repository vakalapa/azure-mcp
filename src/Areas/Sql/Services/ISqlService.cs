// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AzureMcp.Areas.Sql.Models;
using AzureMcp.Options;

namespace AzureMcp.Areas.Sql.Services;

public interface ISqlService
{
    /// <summary>
    /// Gets a SQL database from Azure SQL Server.
    /// </summary>
    /// <param name="serverName">The name of the SQL server</param>
    /// <param name="databaseName">The name of the database</param>
    /// <param name="resourceGroup">The resource group name</param>
    /// <param name="subscription">The subscription ID or name</param>
    /// <param name="retryPolicy">Optional retry policy options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The SQL database information, or null if not found</returns>
    Task<SqlDatabase?> GetDatabaseAsync(
        string serverName,
        string databaseName,
        string resourceGroup,
        string subscription,
        RetryPolicyOptions? retryPolicy,
        CancellationToken cancellationToken = default);
}

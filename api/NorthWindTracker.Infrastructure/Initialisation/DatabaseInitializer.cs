using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NorthWindTracker.Infrastructure.Initialisation;

public class DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
{
    public async Task EnsureCreatedAsync()
    {
        var connectionString = configuration.GetConnectionString("NorthwindDb")!;
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        var exists = await DatabaseExistsAsync(connection);
        if (exists)
        {
            logger.LogInformation("Northwind database already exists, skipping initialisation");
            return;
        }

        logger.LogInformation("Northwind database not found, running setup script...");
        await RunScriptAsync(connection);
        logger.LogInformation("Northwind database created successfully");
    }

    private static async Task<bool> DatabaseExistsAsync(SqlConnection connection)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = 'Northwind'";
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    private static async Task RunScriptAsync(SqlConnection connection)
    {
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "instnwnd.sql");
        scriptPath = Path.GetFullPath(scriptPath);

        var script = await File.ReadAllTextAsync(scriptPath);

        var batches = script.Split(["\r\nGO", "\nGO"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var batch in batches)
        {
            var trimmed = batch.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = trimmed;
            cmd.CommandTimeout = 120;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}

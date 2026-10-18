using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;

namespace projectFamilyTasks.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string InitialParentsPassword = "0000000";

    private readonly string _connectionString = BuildTemporaryConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Default", _connectionString);
        builder.UseSetting("JwtSettings:Key", "integration-tests-signing-key-with-32-characters");
        builder.UseSetting("Seed:InitialParentsPassword", InitialParentsPassword);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            DropTemporaryDatabase();
    }

    private static string BuildTemporaryConnectionString()
    {
        var baseConnection = Environment.GetEnvironmentVariable(PostgresFactAttribute.ConnectionVariable)
                             ?? throw new InvalidOperationException("The test database connection is not set.");
        var builder = new NpgsqlConnectionStringBuilder(baseConnection)
        {
            Database = $"familytasks_it_{Guid.NewGuid():N}"
        };
        return builder.ConnectionString;
    }

    private void DropTemporaryDatabase()
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        var database = builder.Database;
        builder.Database = "postgres";
        NpgsqlConnection.ClearAllPools();

        using var connection = new NpgsqlConnection(builder.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"DROP DATABASE IF EXISTS \"{database}\" WITH (FORCE)";
        command.ExecuteNonQuery();
    }
}

namespace projectFamilyTasks.Tests.Integration;

public sealed class PostgresFactAttribute : FactAttribute
{
    public const string ConnectionVariable = "TEST_DB_CONNECTION";

    public PostgresFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable)))
            Skip = $"Set {ConnectionVariable} to a PostgreSQL connection string to run the integration tests.";
    }
}

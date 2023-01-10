using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using OnlineShop.Configuration;

namespace OnlineShop.Data;

public class DatabaseInitializer
{
    private readonly IOptions<DatabaseConfiguration> _databaseConfiguration;

    public DatabaseInitializer(IOptions<DatabaseConfiguration> databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration;
    }
    
    public async Task InitializeAsync()
    {
        var migrations = await GetMigrations();

        foreach (var db in _databaseConfiguration.Value.Databases)
        {
            var connectionFactory = db.CreateDbConnectionFactory();
            await using var connection = await connectionFactory.CreateConnectionAsync();
            await ExecuteMigrations(connection, migrations);
        }
    }

    private static async Task ExecuteMigrations(IDbConnection dbConnection, List<string> migrations)
    {
        foreach (var migration in migrations)
        {
            await dbConnection.ExecuteAsync(migration);
        }
    }

    private static async Task<List<string>> GetMigrations()
    {
        var migrations = new List<string>();
        foreach (var file in Directory.GetFiles("./Migrations"))
        {
            var sql = await File.ReadAllTextAsync(file);
            migrations.Add(sql);
        }

        return migrations;
    }
}
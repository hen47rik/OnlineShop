using Dapper;

namespace OnlineShop.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        foreach (var file in Directory.GetFiles("./Migrations"))
        {
            var sql = await File.ReadAllTextAsync(file);
            await connection.ExecuteAsync(sql);
        }
    }
}
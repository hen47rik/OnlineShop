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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = await File.ReadAllTextAsync("./online_shop.sql");
        await connection.ExecuteAsync(sql);
    }
}
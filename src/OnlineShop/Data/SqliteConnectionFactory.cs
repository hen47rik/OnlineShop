using System.Data.Common;
using System.Data.SQLite;

namespace OnlineShop.Data;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<DbConnection> CreateConnectionAsync()
    {
        var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
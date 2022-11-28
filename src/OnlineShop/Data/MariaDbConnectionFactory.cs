using System.Data.Common;
using MySql.Data.MySqlClient;

namespace OnlineShop.Data;

public class MariaDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MariaDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<DbConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
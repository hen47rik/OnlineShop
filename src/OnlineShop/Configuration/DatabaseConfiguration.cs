using OnlineShop.Data;

namespace OnlineShop.Configuration;

public class DatabaseConfiguration
{
    public required List<DbConfig> Databases { get; set; }
    public required string FileStorageLocation { get; set; }
}

public class DbConfig
{
    public required string Name { get; set; }
    public required string ConnectionString { get; set; }
    public required DbDialect DbDialect { get; set; }

    public required string Secret { get; set; }

    public IDbConnectionFactory CreateDbConnectionFactory()
    {
        return DbDialect switch
        {
            DbDialect.Sqlite => new SqliteConnectionFactory(ConnectionString),
            DbDialect.MariaDb => new MariaDbConnectionFactory(ConnectionString),
            _ => throw new ArgumentOutOfRangeException($"""There is no implmentation of the "{DbDialect}" SQL dialect configured""")
        };
    }
}

public enum DbDialect
{
    Sqlite,
    MariaDb
}
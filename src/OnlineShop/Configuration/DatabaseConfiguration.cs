namespace OnlineShop.Configuration;

public class DatabaseConfiguration
{
    public required string ActiveDb { get; set; }
    public required List<DbConfig> Databases { get; set; }
}

public class DbConfig
{
    public required string Name { get; set; }
    public required string ConnectionString { get; set; }
    public required DbDialect DbDialect { get; set; }
}

public enum DbDialect
{
    Sqlite,
    MariaDb
}
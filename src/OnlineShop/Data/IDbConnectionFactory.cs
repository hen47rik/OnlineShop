using System.Data.Common;

namespace OnlineShop.Data;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync();
}
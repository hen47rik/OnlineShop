using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data;

public class DatabaseContextFactory : IDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext()
    {
        return new DatabaseContext();
    }
}
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models;

public class DatabaseContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}
using Dapper;
using OnlineShop.Models;

namespace OnlineShop.Data;

public class MySqlDatabase : Database<MySqlDatabase>
{
    public Table<Product> Products { get; set; }
}
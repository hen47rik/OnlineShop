using Dapper;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Services;

public class ProductService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ProductService(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Product>("select * from product p where p.id = ;")
    }
    
    public async Task<List<Product>> GetAllProductsAsync()
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        await connection.QueryAsync<Product>("select * from online_shop.product;");
        await connection.QueryAsync<Product>("SELECT *\nFROM ;")
        
        throw new NotImplementedException();
    }

    public async Task<bool> CreateProduct(Product product)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        await connection.ExecuteScalarAsync<Product>("")
        
        var db = MySqlDatabase.Init(connection, 2);
        await db.Products.InsertAsync(product);
    }
}
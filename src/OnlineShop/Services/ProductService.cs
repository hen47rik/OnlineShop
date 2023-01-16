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
        return await connection.QueryFirstOrDefaultAsync<Product>("select * from product p where p.id = @id;",
            new { id });
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var res = await connection.QueryAsync<Product>("select * from product;");

        return res.ToList();
    }

    public async Task<List<Product>> GetAllProductsIncludingUserAsync()
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        const string sql = """
                select * from product 
                        INNER JOIN order_product op ON product.id = op.product 
                        INNER join `order` o ON op.`order` = o.id 
                        INNER join user u on o.user = u.id;
                """;
        
        var products = await connection.QueryAsync<Product, Order, User, Product>(sql, (product, order, user) =>
        {
            product.Orders.Add(order);
            return product;
        }, splitOn: "order, o.id");

        var result = products.GroupBy(p => p.Id).Select(g =>
        {
            var groupedProduct = g.First();
            groupedProduct.Orders = g.Select(p => p.Orders.Single()).ToList();
            return groupedProduct;
        }).ToList();
        
        return res.ToList();
    }

    public async Task<bool> CreateProduct(Product product)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var res = await connection.ExecuteAsync(@"INSERT INTO product (name, description, images, amount, price) 
VALUES (@name, @description, @images, @amount, @price)",
            new { product.Name, product.Description, product.Images, product.Amount, product.Price });

        return res == 1;
    }

    public async Task<bool> DeleteProduct(int id)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var res = await connection.ExecuteAsync("delete from product where id = @id;", new { id });

        return res == 1;
    }
}
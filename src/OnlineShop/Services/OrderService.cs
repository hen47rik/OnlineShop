using System.Data;
using Dapper;
using OnlineShop.Data;
using OnlineShop.Exceptions;
using OnlineShop.Models;

namespace OnlineShop.Services;

public class OrderService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly UserService _userService;

    public OrderService(IDbConnectionFactory dbConnectionFactory, UserService userService)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _userService = userService;
    }

    public async Task AddProductToOrder(int productId)
    {
        var user = await _userService.GetUser();

        if (user is null)
            throw new BadRequestException("User must be logged in to order a product");
        
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var order = await GetOrder(connection, user.Id);

        if (order is null)
            await connection.ExecuteAsync(@"INSERT INTO `order` (user) VALUES (@id)", new {user.Id});

        order = await GetOrder(connection, user.Id);

        if (order is null)
            throw new BadRequestException("Could not create Order");

        await connection.ExecuteAsync(
            "INSERT INTO order_product (`order`, product) VALUES (@orderId, @productId)", new {orderId = order.Id, productId});
    }

    private async Task<Order?> GetOrder(IDbConnection dbConnection, int userId)
    {
        return await dbConnection.QueryFirstOrDefaultAsync<Order>(
            "select id from `order` o where o.user = @id AND o.isCompleted = FALSE;", new { id = userId });
    }
}
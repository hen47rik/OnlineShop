using System.Security.Claims;
using Dapper;
using OnlineShop.Data;
using OnlineShop.Exceptions;
using OnlineShop.Models;

namespace OnlineShop.Services;

public class UserService
{
    private readonly AuthService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserService(AuthService authService, IHttpContextAccessor httpContextAccessor,
        IDbConnectionFactory dbConnectionFactory)
    {
        _authService = authService;
        _httpContextAccessor = httpContextAccessor;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task RegisterUser(string email, string password)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var existingUser =
            await connection.QueryFirstOrDefaultAsync<User>("select * from user u where u.email = @email;",
                new { email });

        if (existingUser is not null)
            throw new BadRequestException("Email already taken");

        var firstUser = await connection.QueryFirstOrDefaultAsync<User>("select * from user;");

        var isAdmin = firstUser is null;

        _authService.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

        await connection.ExecuteAsync(
            "insert into user (email, passwordHash, passwordSalt, isAdmin) values (@email, @passwordHash, @passwordSalt, @isAdmin);",
            new { email, passwordHash, passwordSalt, isAdmin });

        _authService.AppendAccessToken(email, isAdmin);
    }

    public async Task LoginUser(string email, string password)
    {
        var user = await GetUserByEmailAsync(email);

        if (!_authService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BadRequestException("Username and password do not match");

        _authService.AppendAccessToken(user.Email, user.IsAdmin);
    }

    private async Task<User> GetUserByEmailAsync(string email)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var user = await connection.QueryFirstOrDefaultAsync<User>("select * from user u where u.email = @email;",
            new { email });
        if (user is null)
            throw new BadRequestException("Usere not found");
        return user;
    }

    public string? GetEmail()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    }

    public async Task<User?> GetUser()
    {
        var email = GetEmail();
        if (email is null)
            return null;
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<User>("select * from user u where u.email = @email;",
            new { email });
    }

    public async Task<User?> GetUserById(int id)
    {
        await using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<User>("select * from user u where u.id = @id;", new { id });
    }
}
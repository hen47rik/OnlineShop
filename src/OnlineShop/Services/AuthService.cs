using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.Configuration;
using static System.Text.Encoding;

namespace OnlineShop.Services;

public class AuthService
{
    private readonly IOptions<AuthConfiguration> _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IOptions<AuthConfiguration> configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string CreateToken(string email, bool isAdmin)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email)
        };

        if (isAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var key = new SymmetricSecurityKey(UTF8.GetBytes(_configuration.Value.Secret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(100),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(UTF8.GetBytes(password));
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    public void AppendAccessToken(string email, bool isAdmin)
    {
        var httpResponse = _httpContextAccessor.HttpContext?.Response;

        if (httpResponse is null)
            throw new Exception("HttpResponse was null");

        httpResponse.Cookies.Append("dasToken", CreateToken(email, isAdmin), new CookieOptions
        {
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Domain = null,
            HttpOnly = true,
            IsEssential = true
        });
        httpResponse.Headers.AccessControlAllowCredentials = "true";
    }
}
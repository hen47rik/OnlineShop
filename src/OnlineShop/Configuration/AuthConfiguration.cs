namespace OnlineShop.Configuration;

public class AuthConfiguration
{
    public required string Secret { get; set; }
    public required string AdministratorPassword { get; set; }
}
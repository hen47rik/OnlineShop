namespace OnlineShop.Models;

public class Order
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
    public User User { get; set; }
    public List<Product> Products { get; set; }
}
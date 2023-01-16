using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

public class HomeController : Controller
{
    private readonly ProductService _productService;
    private readonly OrderService _orderService;
    private readonly UserService _userService;

    public HomeController(ProductService productService, OrderService orderService, UserService userService)
    {
        _productService = productService;
        _orderService = orderService;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["products"] = await _productService.GetAllProductsIncludingUserAsync();
        ViewData["user"] =  await _userService.GetUser();
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> AddToOrder(int id)
    {
        await _orderService.AddProductToOrder(id);
        return Redirect("/");
    }
}
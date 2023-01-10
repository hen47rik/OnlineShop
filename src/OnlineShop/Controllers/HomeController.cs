using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

public class HomeController : Controller
{
    private readonly ProductService _productService;

    public HomeController(ProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["products"] = await _productService.GetAllProductsAsync();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
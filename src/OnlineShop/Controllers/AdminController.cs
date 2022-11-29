using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

public class AdminController : Controller
{
    private readonly ProductService _productService;

    public AdminController(ProductService productService)
    {
        _productService = productService;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        ViewData["products"] = await _productService.GetAllProductsAsync();
        return View("Index");
    }

    public async Task<IActionResult> CreateProduct(string name, string description, string images, int amount, int price)
    {
        await _productService.CreateProduct(new Product
        {
            Name = name,
            Description = description,
            Amount = amount,
            Images = images,
            Price = price
        });

        return await Index();
    }

    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProduct(id);
        return await Index();
    }
}
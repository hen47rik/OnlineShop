using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineShop.Configuration;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ProductService _productService;
    private readonly IOptions<DatabaseConfiguration> _dbOptions;
    private readonly DatabaseService _databaseService;

    public AdminController(ProductService productService, IOptions<DatabaseConfiguration> dbOptions, DatabaseService databaseService)
    {
        _productService = productService;
        _dbOptions = dbOptions;
        _databaseService = databaseService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["products"] = await _productService.GetAllProductsAsync();
        var activeDb = _databaseService.FileStorage.ActiveDb;
        ViewData["dbs"] = _dbOptions.Value.Databases.Select(x =>
        {
            var str = x.Name;

            if (x.Name == activeDb)
                str += " (active)";

            return str;
        }).ToList();
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

        return Redirect("/Admin");
    }

    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProduct(id);
        return Redirect("/Admin");
    }

    public async Task<IActionResult> ChangeDb(string name)
    {
        _databaseService.FileStorage.ActiveDb = name;
        await _databaseService.SaveChangesAsync();
        return Redirect("/");
    }
}
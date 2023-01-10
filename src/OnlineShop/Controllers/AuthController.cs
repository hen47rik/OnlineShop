using Microsoft.AspNetCore.Mvc;
using OnlineShop.Services;

namespace OnlineShop.Controllers;

public class AuthController : Controller
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View("PostLogin");
    }

    public IActionResult Login()
    {
        return View("PostLogin");
    }

    public IActionResult Register()
    {
        return View("PostRegister");
    }

    public async Task<IActionResult> PostLogin(string email, string password)
    {
        await _userService.LoginUser(email, password);
        return Redirect("/");
    }

    public async Task<IActionResult> PostRegister(string email, string password)
    {
        await _userService.RegisterUser(email, password);
        return Redirect("/");
    }
}
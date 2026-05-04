using Microsoft.AspNetCore.Mvc;
using PosInventorySystem.Services;

namespace PosInventorySystem.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");
            
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}

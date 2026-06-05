using Microsoft.AspNetCore.Mvc;
// using ProjIS.Models.ViewModels;

namespace ProjIS.Controllers;

public class AuthController : Controller
{
    [HttpGet("/auth/airline")]
    public IActionResult AirlineLogin()
    {
        return View();
    }

    // [HttpPost("/auth/airline")]
    // [ValidateAntiForgeryToken]
    // public IActionResult AirlineLogin(AirlineLoginViewModel model)
    // {
    //     if (!ModelState.IsValid)
    //         return View(model);

    //     // TODO: check name + password against database
    //     // Example:
    //     // var airline = _db.Airlines.FirstOrDefault(a => a.Name == model.Name);
    //     // if (airline == null || !BCrypt.Verify(model.Password, airline.PasswordHash))
    //     // {
    //     //     TempData["Error"] = "Invalid airline name or password.";
    //     //     return View(model);
    //     // }

    //     TempData["Error"] = "Invalid airline name or password.";
    //     return View(model);
    // }

    [HttpGet("/auth/staff")]
    public IActionResult StaffLogin()
    {
        return View();
    }
}
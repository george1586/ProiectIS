using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjIS.Data;
using ProjIS.Models;
using ProjIS.Models.ViewModels;
using System.Security.Claims;

namespace ProjIS.Controllers;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuthController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet("/auth/airline")]
    public IActionResult AirlineLogin()
    {
        return View();
    }

    [HttpPost("/auth/airline")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AirlineLogin(AirlineLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var airline = await _db.Airlines
            .FirstOrDefaultAsync(a => a.Name == model.Name);

        if (airline is null || !BCrypt.Net.BCrypt.Verify(model.Password, airline.PasswordHash))
        {
            TempData["Error"] = "Invalid airline name or password.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, airline.Id.ToString()),
            new Claim(ClaimTypes.Name, airline.Name),
            new Claim(ClaimTypes.Role, "Airline")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Flight");
    }

    [HttpPost("/auth/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/auth/seed-airline")]
    public async Task<IActionResult> SeedAirline()
    {
        const string name = "TAROM";
        const string password = "test123";

        var exists = await _db.Airlines.AnyAsync(a => a.Name == name);
        if (exists)
            return Content($"Airline '{name}' already exists. Login with password: {password}");

        _db.Airlines.Add(new Airline
        {
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return Content($"Created airline '{name}' with password '{password}'. You can now log in.");
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjIS.Data;

namespace ProjIS.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> DbTest()
    {
        var count = await _db.Airlines.CountAsync();
        return Content($"Connected. Airlines in DB: {count}");
    }
}
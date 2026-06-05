using Microsoft.AspNetCore.Mvc;

namespace ProjIS.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
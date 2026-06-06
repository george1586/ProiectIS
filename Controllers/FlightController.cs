using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjIS.Data;
using ProjIS.Models;
using ProjIS.Models.ViewModels;
using System.Security.Claims;

namespace ProjIS.Controllers;

[Authorize(Roles = "Airline")]
public class FlightController : Controller
{
    private readonly ApplicationDbContext _db;

    public FlightController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int CurrentAirlineId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var flights = await _db.Flights
            .Where(f => f.AirlineId == CurrentAirlineId)
            .OrderBy(f => f.RouteCode)
            .ToListAsync();

        return View(flights);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new FlightCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FlightCreateViewModel model)
    {
        if (model.FlightType == FlightType.Seasonal)
        {
            if (model.SeasonStart is null || model.SeasonEnd is null)
                ModelState.AddModelError("", "Seasonal flights require start and end dates.");
            else if (model.SeasonEnd < model.SeasonStart)
                ModelState.AddModelError("", "Season end must be after season start.");
        }
        else
        {
            model.SeasonStart = null;
            model.SeasonEnd = null;
        }

        if (model.DepartureCity.Equals(model.DestinationCity, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError("", "Departure and destination cities must be different.");

        if (!ModelState.IsValid)
            return View(model);

        var flight = new Flight
        {
            RouteCode        = model.RouteCode,
            PlaneModel       = model.PlaneModel,
            DepartureCity    = model.DepartureCity,
            DestinationCity  = model.DestinationCity,
            EconomySeats     = model.EconomySeats,
            BusinessSeats    = model.BusinessSeats,
            FirstClassSeats  = model.FirstClassSeats,
            EconomyPrice     = model.EconomyPrice,
            BusinessPrice    = model.BusinessPrice,
            FirstClassPrice  = model.FirstClassPrice,
            FlightType       = model.FlightType,
            DepartureTime    = model.DepartureTime,
            DaysOfWeek       = model.DaysOfWeek,
            SeasonStart      = model.SeasonStart,
            SeasonEnd        = model.SeasonEnd,
            AirlineId        = CurrentAirlineId,
            CreatedAt        = DateTime.UtcNow
        };

        _db.Flights.Add(flight);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Flight {flight.RouteCode} added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var flight = await _db.Flights
            .FirstOrDefaultAsync(f => f.Id == id && f.AirlineId == CurrentAirlineId);

        if (flight is null)
            return NotFound();

        _db.Flights.Remove(flight);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Flight {flight.RouteCode} deleted.";
        return RedirectToAction(nameof(Index));
    }
}
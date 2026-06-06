using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjIS.Data;
using ProjIS.Models;
using ProjIS.Models.ViewModels;

namespace ProjIS.Controllers;

public class PassengerController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ReservationPriceCalculator _priceCalculator = new();

    public PassengerController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Results(FlightSearchViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.DepartureCity))
        {
            ModelState.AddModelError("", "Departure city is required.");
        }

        if (string.IsNullOrWhiteSpace(model.DestinationCity))
        {
            ModelState.AddModelError("", "Destination city is required.");
        }

        if (model.IsRoundTrip && model.ReturnDate == null)
        {
            ModelState.AddModelError("", "Return date is required for round trip.");
        }

        if (model.IsRoundTrip && model.ReturnDate < model.DepartureDate)
        {
            ModelState.AddModelError("", "Return date must be after departure date.");
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please complete the search form correctly.";
            return RedirectToAction("Index", "Home");
        }

        var outboundFlights = await _db.Flights
            .Include(f => f.Airline)
            .Where(f =>
                f.DepartureCity.ToLower() == model.DepartureCity.ToLower() &&
                f.DestinationCity.ToLower() == model.DestinationCity.ToLower())
            .ToListAsync();

        outboundFlights = outboundFlights
            .Where(f => IsFlightAvailableOnDate(f, model.DepartureDate))
            .ToList();

        var returnFlights = new List<Flight>();

        if (model.IsRoundTrip && model.ReturnDate != null)
        {
            returnFlights = await _db.Flights
                .Include(f => f.Airline)
                .Where(f =>
                    f.DepartureCity.ToLower() == model.DestinationCity.ToLower() &&
                    f.DestinationCity.ToLower() == model.DepartureCity.ToLower())
                .ToListAsync();

            returnFlights = returnFlights
                .Where(f => IsFlightAvailableOnDate(f, model.ReturnDate.Value))
                .ToList();
        }

        ViewBag.Search = model;
        ViewBag.ReturnFlights = returnFlights;

        return View(outboundFlights);
    }

    [HttpGet]
    public async Task<IActionResult> Book(
        int outboundFlightId,
        DateOnly outboundDate,
        int? returnFlightId,
        DateOnly? returnDate)
    {
        var outboundFlight = await _db.Flights
            .Include(f => f.Airline)
            .FirstOrDefaultAsync(f => f.Id == outboundFlightId);

        if (outboundFlight == null)
        {
            return NotFound();
        }

        Flight? returnFlight = null;

        if (returnFlightId != null)
        {
            returnFlight = await _db.Flights
                .Include(f => f.Airline)
                .FirstOrDefaultAsync(f => f.Id == returnFlightId);
        }

        var model = new BookingViewModel
        {
            OutboundFlightId = outboundFlightId,
            ReturnFlightId = returnFlightId,
            OutboundDate = outboundDate,
            ReturnDate = returnDate,
            OutboundFlight = outboundFlight,
            ReturnFlight = returnFlight,
            ReturnClass = returnFlight != null ? FlightClass.Economy : null
        };

        model.EstimatedPrice = _priceCalculator.Calculate(
            outboundFlight,
            model.OutboundClass,
            returnFlight,
            model.ReturnClass,
            model.AdultsCount,
            model.ChildrenCount,
            model.SeniorsCount,
            model.HasMeal,
            model.HasExtraLuggage,
            model.OutboundDate,
            model.ReturnDate);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookingViewModel model)
    {
        if (model.AdultsCount + model.ChildrenCount + model.SeniorsCount <= 0)
        {
            ModelState.AddModelError("", "At least one passenger is required.");
        }

        var outboundFlight = await _db.Flights
            .FirstOrDefaultAsync(f => f.Id == model.OutboundFlightId);

        if (outboundFlight == null)
        {
            return NotFound();
        }

        Flight? returnFlight = null;

        if (model.ReturnFlightId != null)
        {
            returnFlight = await _db.Flights
                .FirstOrDefaultAsync(f => f.Id == model.ReturnFlightId);
        }

        if (!ModelState.IsValid)
        {
            model.OutboundFlight = outboundFlight;
            model.ReturnFlight = returnFlight;

            model.EstimatedPrice = _priceCalculator.Calculate(
                outboundFlight,
                model.OutboundClass,
                returnFlight,
                model.ReturnClass,
                model.AdultsCount,
                model.ChildrenCount,
                model.SeniorsCount,
                model.HasMeal,
                model.HasExtraLuggage,
                model.OutboundDate,
                model.ReturnDate);

            return View(model);
        }

        decimal totalPrice = _priceCalculator.Calculate(
            outboundFlight,
            model.OutboundClass,
            returnFlight,
            model.ReturnClass,
            model.AdultsCount,
            model.ChildrenCount,
            model.SeniorsCount,
            model.HasMeal,
            model.HasExtraLuggage,
            model.OutboundDate,
            model.ReturnDate);

        var reservation = new Reservation
        {
            PassengerName = model.PassengerName,
            Phone = model.Phone,

            AdultsCount = model.AdultsCount,
            ChildrenCount = model.ChildrenCount,
            SeniorsCount = model.SeniorsCount,

            OutboundFlightId = model.OutboundFlightId,
            OutboundDate = model.OutboundDate,
            OutboundClass = model.OutboundClass,

            ReturnFlightId = model.ReturnFlightId,
            ReturnDate = model.ReturnDate,
            ReturnClass = model.ReturnClass,

            HasMeal = model.HasMeal,
            HasExtraLuggage = model.HasExtraLuggage,

            TotalPrice = totalPrice,
            PaymentMethod = model.PaymentMethod,
            PaymentStatus = model.PaymentMethod == PaymentMethod.Card
                ? PaymentStatus.Paid
                : PaymentStatus.Pending,

            CreatedAt = DateTime.UtcNow
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Confirmation), new { id = reservation.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var reservation = await _db.Reservations
            .Include(r => r.OutboundFlight)
            .Include(r => r.ReturnFlight)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    private bool IsFlightAvailableOnDate(Flight flight, DateOnly date)
    {
        int dayOfWeek = ((int)date.DayOfWeek + 6) % 7 + 1;

        bool correctDay = flight.DaysOfWeek.Contains(dayOfWeek);

        if (!correctDay)
        {
            return false;
        }

        if (flight.FlightType == FlightType.Seasonal)
        {
            if (flight.SeasonStart == null || flight.SeasonEnd == null)
            {
                return false;
            }

            return date >= flight.SeasonStart && date <= flight.SeasonEnd;
        }

        return true;
    }
}
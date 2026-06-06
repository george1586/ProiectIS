using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjIS.Data;
using ProjIS.Models;
using ProjIS.Models.ViewModels;
using System.Security.Claims;

namespace ProjIS.Controllers;

public class AirportStaffController : Controller
{
    private readonly ApplicationDbContext _db;

    public AirportStaffController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int CurrentStaffId
    {
        get
        {
            string? staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (staffId == null)
            {
                return 0;
            }

            return int.Parse(staffId);
        }
    }

    [HttpGet("/airport-staff")]
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("AirportStaff"))
        {
            return RedirectToAction(nameof(Dashboard));
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet("/airport-staff/login")]
    public IActionResult Login()
    {
        return View(new AirportStaffLoginViewModel());
    }

    [HttpPost("/airport-staff/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AirportStaffLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var staff = await _db.AirportStaff
            .FirstOrDefaultAsync(s => s.PersonalCode == model.PersonalCode);

        if (staff == null)
        {
            TempData["Error"] = "Invalid airport staff code.";
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
            new Claim(ClaimTypes.Name, staff.FullName),
            new Claim(ClaimTypes.Role, "AirportStaff")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction(nameof(Dashboard));
    }

    [Authorize(Roles = "AirportStaff")]
    [HttpGet("/airport-staff/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var pendingCashCount = await _db.Reservations
            .CountAsync(r =>
                r.PaymentMethod == PaymentMethod.Cash &&
                r.PaymentStatus == PaymentStatus.Pending);

        var validatedCashCount = await _db.Reservations
            .CountAsync(r =>
                r.PaymentMethod == PaymentMethod.Cash &&
                r.PaymentStatus == PaymentStatus.Paid);

        ViewBag.PendingCashCount = pendingCashCount;
        ViewBag.ValidatedCashCount = validatedCashCount;
        ViewBag.StaffName = User.Identity?.Name ?? "Airport Staff";

        return View();
    }

    [Authorize(Roles = "AirportStaff")]
    [HttpGet("/airport-staff/cash-payments")]
    public async Task<IActionResult> CashPayments()
    {
        var reservations = await _db.Reservations
            .Include(r => r.OutboundFlight)
            .Include(r => r.ReturnFlight)
            .Where(r =>
                r.PaymentMethod == PaymentMethod.Cash &&
                r.PaymentStatus == PaymentStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        return View(reservations);
    }

    [Authorize(Roles = "AirportStaff")]
    [HttpPost("/airport-staff/validate-payment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidatePayment(int reservationId)
    {
        var reservation = await _db.Reservations
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            return NotFound();
        }

        var validator = new CashPaymentValidator();

        if (!validator.CanValidate(reservation))
        {
            TempData["Error"] = "This payment cannot be validated.";
            return RedirectToAction(nameof(CashPayments));
        }

        validator.Validate(reservation, CurrentStaffId);

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Reservation #{reservation.Id} was validated.";
        return RedirectToAction(nameof(CashPayments));
    }

    [Authorize(Roles = "AirportStaff")]
    [HttpGet("/airport-staff/validated-payments")]
    public async Task<IActionResult> ValidatedPayments()
    {
        var reservations = await _db.Reservations
            .Include(r => r.OutboundFlight)
            .Include(r => r.ReturnFlight)
            .Include(r => r.ValidatedByStaffNavigation)
            .Where(r =>
                r.PaymentMethod == PaymentMethod.Cash &&
                r.PaymentStatus == PaymentStatus.Paid)
            .OrderByDescending(r => r.ValidatedAt)
            .ToListAsync();

        return View(reservations);
    }

    [HttpPost("/airport-staff/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/airport-staff/seed")]
    public async Task<IActionResult> SeedStaff()
    {
        const string personalCode = "STAFF2026";
        const string fullName = "Airport Staff Test";

        var exists = await _db.AirportStaff
            .AnyAsync(s => s.PersonalCode == personalCode);

        if (exists)
        {
            return Content($"Airport staff already exists. Login with code: {personalCode}");
        }

        _db.AirportStaff.Add(new AirportStaff
        {
            PersonalCode = personalCode,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return Content($"Created airport staff '{fullName}' with code '{personalCode}'.");
    }
}
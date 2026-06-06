using ProjIS.Models;

namespace ProjIS.Models;

public enum FlightClass
{
    Economy,
    Business,
    First
}

public enum PaymentMethod
{
    Card,
    Cash
}

public enum PaymentStatus
{
    Pending,
    Paid
}

public class Reservation
{
    public int Id { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int AdultsCount { get; set; }
    public int ChildrenCount { get; set; }
    public int SeniorsCount { get; set; }
    public int OutboundFlightId { get; set; }
    public Flight? OutboundFlight { get; set; }
    public DateOnly OutboundDate { get; set; }
    public FlightClass OutboundClass { get; set; }
    public int? ReturnFlightId { get; set; }
    public Flight? ReturnFlight { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public FlightClass? ReturnClass { get; set; }
    public bool HasMeal { get; set; }
    public bool HasExtraLuggage { get; set; }
    public decimal TotalPrice { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public int? ValidatedByStaff { get; set; }
    public AirportStaff? ValidatedByStaffNavigation { get; set; }
    public DateTime? ValidatedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
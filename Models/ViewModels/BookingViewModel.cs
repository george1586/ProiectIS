using System.ComponentModel.DataAnnotations;
using ProjIS.Models;

namespace ProjIS.Models.ViewModels;

public class BookingViewModel
{
    public int OutboundFlightId { get; set; }
    public int? ReturnFlightId { get; set; }

    public DateOnly OutboundDate { get; set; }
    public DateOnly? ReturnDate { get; set; }

    [Required]
    public string PassengerName { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Range(0, 20)]
    public int AdultsCount { get; set; } = 1;

    [Range(0, 20)]
    public int ChildrenCount { get; set; }

    [Range(0, 20)]
    public int SeniorsCount { get; set; }

    public FlightClass OutboundClass { get; set; } = FlightClass.Economy;
    public FlightClass? ReturnClass { get; set; }

    public bool HasMeal { get; set; }
    public bool HasExtraLuggage { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;

    public decimal EstimatedPrice { get; set; }

    public Flight? OutboundFlight { get; set; }
    public Flight? ReturnFlight { get; set; }
}
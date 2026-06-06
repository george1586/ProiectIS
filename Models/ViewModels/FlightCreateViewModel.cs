using System.ComponentModel.DataAnnotations;
using ProjIS.Models;


namespace ProjIS.Models.ViewModels;

public class FlightCreateViewModel
{
    [Required, StringLength(20)]
    [Display(Name = "Route code")]
    public string RouteCode { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Plane model")]
    public string PlaneModel { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Departure city")]
    public string DepartureCity { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Destination city")]
    public string DestinationCity { get; set; } = string.Empty;

    [Range(0, 1000)] public int EconomySeats { get; set; }
    [Range(0, 1000)] public int BusinessSeats { get; set; }
    [Range(0, 1000)] public int FirstClassSeats { get; set; }

    [Range(0, 100000)] public decimal EconomyPrice { get; set; }
    [Range(0, 100000)] public decimal BusinessPrice { get; set; }
    [Range(0, 100000)] public decimal FirstClassPrice { get; set; }

    [Required]
    public FlightType FlightType { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly DepartureTime { get; set; }

    [Required, MinLength(1, ErrorMessage = "Select at least one day.")]
    public int[] DaysOfWeek { get; set; } = Array.Empty<int>();

    [DataType(DataType.Date)]
    public DateOnly? SeasonStart { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? SeasonEnd { get; set; }
}
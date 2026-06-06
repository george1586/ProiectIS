using System.ComponentModel.DataAnnotations;

namespace ProjIS.Models.ViewModels;

public class FlightSearchViewModel
{
    [Required]
    public string DepartureCity { get; set; } = string.Empty;

    [Required]
    public string DestinationCity { get; set; } = string.Empty;

    [Required]
    public DateOnly DepartureDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public bool IsRoundTrip { get; set; }

    public DateOnly? ReturnDate { get; set; }

    [Range(1, 20)]
    public int Passengers { get; set; } = 1;
}
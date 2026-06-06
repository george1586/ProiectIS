namespace ProjIS.Models;

public enum FlightType
{
    Regular,
    Seasonal
}

public class Flight
{
    public int Id { get; set; }
    public string RouteCode { get; set; } = string.Empty;
    public string PlaneModel { get; set; } = string.Empty;
    public string DepartureCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public int EconomySeats { get; set; }
    public int BusinessSeats { get; set; }
    public int FirstClassSeats { get; set; }
    public decimal EconomyPrice { get; set; }
    public decimal BusinessPrice { get; set; }
    public decimal FirstClassPrice { get; set; }
    public FlightType FlightType { get; set; }
    public TimeOnly DepartureTime { get; set; }
    public int[] DaysOfWeek { get; set; } = Array.Empty<int>();   // 1=Mon..7=Sun
    public DateOnly? SeasonStart { get; set; }
    public DateOnly? SeasonEnd { get; set; }
    public int AirlineId { get; set; }
    public Airline? Airline { get; set; }
    public DateTime CreatedAt { get; set; }
}
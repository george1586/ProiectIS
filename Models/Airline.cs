namespace ProjIS.Models;
 
public class Airline
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
 
    // Navigation: flights owned by this airline
    public List<Flight> Flights { get; set; } = new();
}

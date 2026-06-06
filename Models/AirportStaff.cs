namespace ProjIS.Models;
 
public class AirportStaff
{
    public int Id { get; set; }
    public string PersonalCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
 

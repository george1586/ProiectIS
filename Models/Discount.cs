namespace ProjIS.Models;
 
public class Discount
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime UpdatedAt { get; set; }
}
 

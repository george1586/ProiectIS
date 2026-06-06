using System.ComponentModel.DataAnnotations;

namespace ProjIS.Models.ViewModels;

public class AirportStaffLoginViewModel
{
    [Required]
    public string PersonalCode { get; set; } = string.Empty;
}
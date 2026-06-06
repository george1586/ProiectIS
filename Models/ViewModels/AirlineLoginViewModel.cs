using System.ComponentModel.DataAnnotations;

namespace ProjIS.Models.ViewModels;

public class AirlineLoginViewModel
{
    [Required(ErrorMessage = "Airline name is required.")]
    [Display(Name = "Airline name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
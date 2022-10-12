using System.ComponentModel.DataAnnotations;

namespace Renting.Models.Account;

public class ApplicationUserLogin
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(15, ErrorMessage = "Password must be at most 15 characters")]
    public string Password { get; set; }
}

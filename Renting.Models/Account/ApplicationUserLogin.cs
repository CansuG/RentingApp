using System.ComponentModel.DataAnnotations;

namespace Renting.Models.Account
{
    public class ApplicationUserLogin
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(8, ErrorMessage = "Must be 5-40 characters")]
        [MaxLength(40, ErrorMessage = "Must be 5-40 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(10, ErrorMessage = "Must be at 10-50 characters")]
        [MaxLength(50, ErrorMessage = "Must be at 10-50 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Password { get; set; }
    }
}

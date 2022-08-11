using System.ComponentModel.DataAnnotations;

namespace Renting.Models.Account
{
    public class ApplicationUserCreate : ApplicationUserLogin
    {

        [MinLength(10, ErrorMessage = "Must be 10-30 characters")]
        [MaxLength(30, ErrorMessage = "Must be 10-30 characters")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(20, ErrorMessage = "Can be at most 30 characters")]
        public string Username { get; set; }

        public string Gender { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Renting.Models.Account;


public class ApplicationUserCreate : ApplicationUserLogin
{
    [Required(ErrorMessage = "Firstname is required")]
    [MaxLength(20, ErrorMessage = "Must be at most 20 characters")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Lastname is required")]
    [MaxLength(20, ErrorMessage = "Must be at most 20 characters")]
    public string? LastName { get; set; }


    [Required(ErrorMessage = "Username is required")]
    [MinLength(5, ErrorMessage = "Must be at least 5 characters")]
    [MaxLength(20, ErrorMessage = "Must be at most 20 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [MaxLength(20, ErrorMessage = "Must be at most 20 characters")]
    public string Gender { get; set; }
    public string? ImageUrl { get; set; }
    public string? PublicId { get; set; }
}

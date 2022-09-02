using System.ComponentModel.DataAnnotations;

namespace Renting.Models.Account;

public class ApplicationUserLogin
{
    
    public string Email { get; set; }
    public string Password { get; set; }
}

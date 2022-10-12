using System.ComponentModel.DataAnnotations;

namespace Renting.Models.Account;

public class ApplicationUserCreate : ApplicationUserLogin
{

    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string Username { get; set; }

    public string Gender { get; set; }

    public int? PhotoId { get; set; }
}

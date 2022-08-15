using System.ComponentModel.DataAnnotations;

namespace Renting.Models.Account
{
    public class ApplicationUserCreate : ApplicationUserLogin
    {

        public string Fullname { get; set; }
        
        public string Username { get; set; }

        public string Gender { get; set; }
    }
}

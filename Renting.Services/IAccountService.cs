using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Services
{
    public interface IAccountService
    {
        public string UniqueForEmail(string name, IList<string> emailsList);
        public string UniqueForUsername(string name, IList<string>usernamesList);
        
    }
}

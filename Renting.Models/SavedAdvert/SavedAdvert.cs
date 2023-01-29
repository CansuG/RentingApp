using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.SavedAdvert
{
    public class SavedAdvert : SavedAdvertCreate
    {
        public int AdvertId { get; set; }
        public int ApplicationUserId { get; set; }
    }
}

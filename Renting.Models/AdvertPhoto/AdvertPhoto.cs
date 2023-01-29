using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.AdvertPhoto
{
    public class AdvertPhoto : AdvertPhotoCreate
    {
        public int PhotoId { get; set; }
        public int AdvertId { get; set; }
        public DateTime AddingDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.Advert
{
    public class Results<T>
    {
        public IEnumerable<T>? Items { get; set; }

        public int TotalCount { get; set; }
    }
}

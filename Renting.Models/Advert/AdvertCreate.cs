using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.Advert;

    public class AdvertCreate
    {
        public int AdvertId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string City { get; set; }
		public string District { get; set; }
		public string Neighbourhood { get; set; }
		public string Rooms { get; set; }
		public decimal Price { get; set; }
		public string FloorArea { get; set; }

    }

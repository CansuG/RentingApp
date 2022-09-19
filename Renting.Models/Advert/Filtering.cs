using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.Advert
{
    public class Filtering
    {
		public string? City { get; set; }
		public string? District { get; set; }
		public string? Neighbourhood { get; set; }
		public string? Rooms { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public int? MinFloorArea { get; set; }
		public int? MaxFloorArea { get; set; }
		public int Page { get; set; } 
		public int PageSize { get; set; } 

	}
}

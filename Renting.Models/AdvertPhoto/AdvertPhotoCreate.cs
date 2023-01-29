using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.AdvertPhoto
{
    public class AdvertPhotoCreate
    {
     
        [Required(ErrorMessage = "ImageURL is required")]
        public string ImageUrl { get; set; }
     
        [Required(ErrorMessage = "PublicId is required")]
        public string PublicId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renting.Models.Advert;

public class AdvertCreate
{
    public int AdvertId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MinLength(5, ErrorMessage = "Must be at least 5 characters")]
    [MaxLength(50, ErrorMessage = "Must be at most 50 characters")]
    public string Title { get; set; }

    [MaxLength(250, ErrorMessage = "Content must be at most 250 characters")]
    public string Content { get; set; }

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; }

    [Required(ErrorMessage = "District is required")]
    public string District { get; set; }

    [Required(ErrorMessage = "Neighbourhood is required")]
    public string Neighbourhood { get; set; }

    [Required(ErrorMessage = "Rooms is required")]
    public string Rooms { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(1, 1000000, ErrorMessage = "Price must be greater than 1")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "FloorArea is required")]
    [Range(1, 1000, ErrorMessage = "FloorArea must be greater than 1")]
    public string FloorArea { get; set; }
}

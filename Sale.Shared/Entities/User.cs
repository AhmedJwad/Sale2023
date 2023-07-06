using Microsoft.AspNetCore.Identity;
using Sale.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sale.Shared.Entities
{
    public class User:IdentityUser
    {
        [Display(Name ="Document")]
        [MaxLength(20 , ErrorMessage ="the field {0} must have a maximum of {1} characters")]
        [Required(ErrorMessage ="the field {0} is required")]
        public string Document { get; set; } = null;
        [Display(Name = "First Name ")]
        [MaxLength(50, ErrorMessage = "the field {0} must have a maximum of {1} characters")]
        [Required(ErrorMessage = "the field {0} is required")]
        public string Firstname { get; set; } = null;

        [Display(Name = "Last Name ")]
        [MaxLength(50, ErrorMessage = "the field {0} must have a maximum of {1} characters")]
        [Required(ErrorMessage = "the field {0} is required")]
        public string LastName { get; set; } = null;

        [Display(Name = "Address")]
        [MaxLength(50, ErrorMessage = "the field {0} must have a maximum of {1} characters")]
        [Required(ErrorMessage = "the field {0} is required")]
        public string Address { get; set; } = null;

        [Display(Name ="Photo")]
        public string? Photo { get; set; }

        [Display(Name ="User type")]
        public UserType UserType { get; set; }

        public City? City { get; set; }

        [Display(Name ="City")]
        [Range(1,int .MaxValue, ErrorMessage ="you must select a {0}")]
        public int CityId { get; set; }
        [Display(Name ="Full Name")]
        public string Fullname => $"{Firstname} {LastName}";

        [Display(Name = "Photo")]
        public string ImageFullPath => Photo == string.Empty
            ? $"http://soccerworldcup.somee.com/images/noimage.png"
            : $"https://localhost:7011/{Photo}";
        public ICollection<TemporalSale>? TemporalSales { get; set; }
    }
}

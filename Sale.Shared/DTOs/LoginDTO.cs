using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sale.Shared.DTOs
{
   public class LoginDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [MinLength(6, ErrorMessage = "The field {0} must have at least {1} characters.")]
        public string Password { get; set; } = null!;

    }
}

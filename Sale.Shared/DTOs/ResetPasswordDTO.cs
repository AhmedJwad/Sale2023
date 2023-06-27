using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sale.Shared.DTOs
{
    public class ResetPasswordDTO
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} field must be between {2} and {1} characters.")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The new password and confirmation are not the same.")]
        [DataType(DataType.Password)]
        [Display(Name = "password confirmation")]
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must have between {2} and {1} characters.")]
        public string ConfirmPassword { get; set; } = null!;

        public string Token { get; set; } = null!;

    }
}

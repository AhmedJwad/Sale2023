using Sale.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Shared.DTOs
{
    public class UserDTO : User
    {
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        [Required(ErrorMessage ="the field {0} is required")]
        [StringLength (20, MinimumLength =6 , ErrorMessage = "The field {0} must have between {2} and {1} characters")]
        public string Password { get; set; } = null;

        [DataType(DataType.Password)]
        [Display(Name = "Password Confirm")]
        [Required(ErrorMessage = "the field {0} is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must have between {2} and {1} characters")]
        [Compare("Password", ErrorMessage = "Password and confirmation are not the same")]
        public string PasswordConfirm { get; set; } = null!;

    }
}
